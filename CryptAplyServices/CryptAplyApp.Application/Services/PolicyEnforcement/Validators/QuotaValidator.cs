using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.PolicyEnforcement.Validators
{
    public class QuotaValidator : IQuotaValidator
    {
        private readonly ILogger<QuotaValidator> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly ConcurrentDictionary<string, OperationCounter> _operationCounters;
        private readonly ConcurrentDictionary<string, DataVolumeCounter> _volumeCounters;

        public QuotaValidator(ILogger<QuotaValidator> logger, IAuditLogger auditLogger)
        {
            _logger = logger;
            _auditLogger = auditLogger;
            _operationCounters = new ConcurrentDictionary<string, OperationCounter>();
            _volumeCounters = new ConcurrentDictionary<string, DataVolumeCounter>();
        }

        public async Task<bool> CheckQuotasAsync(KeyOperationContext context, UsageQuotas controls)
        {
            try
            {
                var appId = context.ApplicationId;
                var counter = _operationCounters.GetOrAdd(appId, _ => new OperationCounter());

                // Check rate limits
                if (!CheckRateLimits(context, controls.OperationRateLimits, counter))
                {
                    await LogQuotaViolationAsync(context, "Rate limit exceeded");
                    return false;
                }

                // Check data volume limits if applicable
                if (controls.DataVolumeLimits != null &&
                    controls.DataVolumeLimits.TryGetValue(context.Operation, out var volumeLimit))
                {
                    var volumeCounter = _volumeCounters.GetOrAdd(appId, _ => new DataVolumeCounter());
                    if (!CheckVolumeLimit(context, volumeLimit, volumeCounter))
                    {
                        await LogQuotaViolationAsync(context, "Data volume limit exceeded");
                        return false;
                    }
                }

                // Check concurrent operation limits
                if (controls.ConcurrentOperationLimits != null &&
                    controls.ConcurrentOperationLimits.TryGetValue(context.Operation, out var concurrentLimit))
                {
                    if (counter.CurrentConcurrent >= concurrentLimit)
                    {
                        await LogQuotaViolationAsync(context, "Concurrent operation limit exceeded");
                        return false;
                    }
                }

                await _auditLogger.LogAsync(new AuditEvent
                {
                    EventType = "QuotaCheck",
                    ApplicationId = appId,
                    Success = true,
                    Timestamp = DateTime.UtcNow,
                    Metadata = new Dictionary<string, string>
                    {
                        { "Operation", context.Operation },
                        { "CurrentRate", counter.GetCurrentRate().ToString() }
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking quotas for {ApplicationId}", context.ApplicationId);
                return false;
            }
        }

        public async Task<QuotaStatus> GetCurrentQuotaStatusAsync(string applicationId)
        {
            var operationCounter = _operationCounters.GetOrAdd(applicationId, _ => new OperationCounter());
            var volumeCounter = _volumeCounters.GetOrAdd(applicationId, _ => new DataVolumeCounter());

            return new QuotaStatus
            {
                CurrentOperationRate = operationCounter.GetCurrentRate(),
                CurrentConcurrentOperations = operationCounter.CurrentConcurrent,
                TotalOperationsToday = operationCounter.GetDailyTotal(),
                DataVolumeToday = volumeCounter.GetDailyVolume(),
                LastUpdated = DateTime.UtcNow
            };
        }

        public async Task RecordOperationAsync(KeyOperationContext context)
        {
            try
            {
                var appId = context.ApplicationId;
                var counter = _operationCounters.GetOrAdd(appId, _ => new OperationCounter());
                var volumeCounter = _volumeCounters.GetOrAdd(appId, _ => new DataVolumeCounter());

                counter.IncrementOperation();
                if (context.Metadata?.TryGetValue("DataSize", out var dataSizeStr) == true &&
                    long.TryParse(dataSizeStr, out var dataSize))
                {
                    volumeCounter.AddVolume(dataSize);
                }

                await _auditLogger.LogAsync(new AuditEvent
                {
                    EventType = "OperationRecorded",
                    ApplicationId = appId,
                    Operation = context.Operation,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording operation for {ApplicationId}", context.ApplicationId);
            }
        }

        private bool CheckRateLimits(KeyOperationContext context, RateLimits limits, OperationCounter counter)
        {
            var currentRate = counter.GetCurrentRate();

            if (currentRate >= limits.OperationsPerSecond)
            {
                return false;
            }

            if (limits.OperationSpecificLimits != null &&
                limits.OperationSpecificLimits.TryGetValue(context.Operation, out var specificLimit) &&
                currentRate >= specificLimit)
            {
                return false;
            }

            return true;
        }

        private bool CheckVolumeLimit(KeyOperationContext context, long volumeLimit, DataVolumeCounter counter)
        {
            if (!context.Metadata?.TryGetValue("DataSize", out var dataSizeStr) == true ||
                !long.TryParse(dataSizeStr, out var dataSize))
            {
                return true; // No data size specified, assume ok
            }

            var currentVolume = counter.GetDailyVolume();
            return (currentVolume + dataSize) <= volumeLimit;
        }

        private async Task LogQuotaViolationAsync(KeyOperationContext context, string reason)
        {
            await _auditLogger.LogAsync(new AuditEvent
            {
                EventType = "QuotaViolation",
                ApplicationId = context.ApplicationId,
                Success = false,
                Timestamp = DateTime.UtcNow,
                Details = new[] { reason }
            });
        }
    }

    public class OperationCounter
    {
        private readonly ConcurrentQueue<DateTime> _operations = new ConcurrentQueue<DateTime>();
        private int _currentConcurrent;
        private readonly object _lock = new object();

        public int CurrentConcurrent => _currentConcurrent;

        public void IncrementOperation()
        {
            lock (_lock)
            {
                _currentConcurrent++;
                _operations.Enqueue(DateTime.UtcNow);
                CleanupOldOperations();
            }
        }

        public void DecrementOperation()
        {
            lock (_lock)
            {
                if (_currentConcurrent > 0)
                {
                    _currentConcurrent--;
                }
            }
        }

        public int GetCurrentRate()
        {
            CleanupOldOperations();
            return _operations.Count;
        }

        public int GetDailyTotal()
        {
            var today = DateTime.UtcNow.Date;
            return _operations.Count(op => op.Date == today);
        }

        private void CleanupOldOperations()
        {
            var threshold = DateTime.UtcNow.AddSeconds(-1);
            while (_operations.TryPeek(out var oldestOp) && oldestOp < threshold)
            {
                _operations.TryDequeue(out _);
            }
        }
    }

    public class DataVolumeCounter
    {
        private readonly ConcurrentDictionary<DateTime, long> _dailyVolumes = new ConcurrentDictionary<DateTime, long>();

        public void AddVolume(long bytes)
        {
            var today = DateTime.UtcNow.Date;
            _dailyVolumes.AddOrUpdate(today,
                bytes,
                (_, existing) => existing + bytes);
            
            // Cleanup old data
            var oldDate = today.AddDays(-7);
            foreach (var key in _dailyVolumes.Keys.Where(k => k < oldDate))
            {
                _dailyVolumes.TryRemove(key, out _);
            }
        }

        public long GetDailyVolume()
        {
            var today = DateTime.UtcNow.Date;
            return _dailyVolumes.GetValueOrDefault(today);
        }
    }

    public class QuotaStatus
    {
        public int CurrentOperationRate { get; set; }
        public int CurrentConcurrentOperations { get; set; }
        public int TotalOperationsToday { get; set; }
        public long DataVolumeToday { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
