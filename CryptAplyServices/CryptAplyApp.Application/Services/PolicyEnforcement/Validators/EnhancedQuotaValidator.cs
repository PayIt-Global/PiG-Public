using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.PolicyEnforcement.Validators
{
    public class EnhancedQuotaValidator : IQuotaValidator
    {
        private readonly ILogger<EnhancedQuotaValidator> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly ConcurrentDictionary<string, AdaptiveQuota> _adaptiveQuotas;
        private readonly ConcurrentDictionary<string, BurstQuota> _burstQuotas;
        private readonly ConcurrentDictionary<string, CostTracker> _costTrackers;

        public EnhancedQuotaValidator(
            ILogger<EnhancedQuotaValidator> logger,
            IAuditLogger auditLogger)
        {
            _logger = logger;
            _auditLogger = auditLogger;
            _adaptiveQuotas = new ConcurrentDictionary<string, AdaptiveQuota>();
            _burstQuotas = new ConcurrentDictionary<string, BurstQuota>();
            _costTrackers = new ConcurrentDictionary<string, CostTracker>();
        }

        public async Task<bool> CheckQuotasAsync(KeyOperationContext context, UsageQuotas controls)
        {
            try
            {
                var enhancedControls = controls as EnhancedQuotaControls;
                if (enhancedControls == null)
                {
                    return await base.CheckQuotasAsync(context, controls);
                }

                // Check adaptive quotas if enabled
                if (enhancedControls.EnableAdaptiveQuotas)
                {
                    var adaptiveQuota = GetAdaptiveQuota(context.ApplicationId, enhancedControls);
                    if (!adaptiveQuota.CheckQuota())
                    {
                        await LogQuotaViolationAsync(context, "Adaptive quota exceeded");
                        return false;
                    }
                }

                // Check burst capacity if enabled
                if (enhancedControls.AllowBursting)
                {
                    var burstQuota = GetBurstQuota(context.ApplicationId, enhancedControls);
                    if (!burstQuota.CheckBurst())
                    {
                        await LogQuotaViolationAsync(context, "Burst quota exceeded");
                        return false;
                    }
                }

                // Check cost-based quotas if enabled
                if (enhancedControls.EnforceBudgetLimits)
                {
                    var costTracker = GetCostTracker(context.ApplicationId, enhancedControls);
                    if (!await costTracker.CheckOperationCostAsync(context.Operation))
                    {
                        await LogQuotaViolationAsync(context, "Daily budget limit exceeded");
                        return false;
                    }
                }

                // Check priority-based throttling
                if (enhancedControls.EnablePriorityBasedThrottling)
                {
                    if (!await CheckPriorityQuotaAsync(context, enhancedControls))
                    {
                        await LogQuotaViolationAsync(context, "Priority-based quota exceeded");
                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking enhanced quotas for {ApplicationId}", context.ApplicationId);
                return false;
            }
        }

        private AdaptiveQuota GetAdaptiveQuota(string applicationId, EnhancedQuotaControls controls)
        {
            return _adaptiveQuotas.GetOrAdd(applicationId, _ => new AdaptiveQuota(
                controls.MinimumQuota,
                controls.MaximumQuota,
                controls.QuotaAdjustmentFactor));
        }

        private BurstQuota GetBurstQuota(string applicationId, EnhancedQuotaControls controls)
        {
            return _burstQuotas.GetOrAdd(applicationId, _ => new BurstQuota(
                controls.BurstCapacity,
                controls.BurstDuration));
        }

        private CostTracker GetCostTracker(string applicationId, EnhancedQuotaControls controls)
        {
            return _costTrackers.GetOrAdd(applicationId, _ => new CostTracker(
                controls.DailyBudget,
                controls.OperationCosts));
        }

        private async Task<bool> CheckPriorityQuotaAsync(
            KeyOperationContext context,
            EnhancedQuotaControls controls)
        {
            if (!controls.OperationPriorities.TryGetValue(context.Operation, out var priority))
            {
                priority = QuotaPriority.Low; // Default priority
            }

            var currentLoad = GetCurrentLoad(context.ApplicationId);
            
            // Higher priority operations are allowed even under high load
            switch (priority)
            {
                case QuotaPriority.Critical:
                    return true;
                case QuotaPriority.High:
                    return currentLoad < 90;
                case QuotaPriority.Medium:
                    return currentLoad < 70;
                case QuotaPriority.Low:
                    return currentLoad < 50;
                case QuotaPriority.Background:
                    return currentLoad < 30;
                default:
                    return false;
            }
        }

        private int GetCurrentLoad(string applicationId)
        {
            // Implementation to get current system load
            return 50; // Placeholder
        }

        private async Task LogQuotaViolationAsync(KeyOperationContext context, string reason)
        {
            await _auditLogger.LogAsync(new AuditEvent
            {
                EventType = "QuotaViolation",
                ApplicationId = context.ApplicationId,
                Success = false,
                Timestamp = DateTime.UtcNow,
                Details = new[] { reason },
                Metadata = new Dictionary<string, string>
                {
                    { "Operation", context.Operation },
                    { "CurrentLoad", GetCurrentLoad(context.ApplicationId).ToString() }
                }
            });
        }
    }

    internal class AdaptiveQuota
    {
        private readonly int _minimumQuota;
        private readonly int _maximumQuota;
        private readonly double _adjustmentFactor;
        private int _currentQuota;
        private readonly object _lock = new object();

        public AdaptiveQuota(int minimumQuota, int maximumQuota, double adjustmentFactor)
        {
            _minimumQuota = minimumQuota;
            _maximumQuota = maximumQuota;
            _adjustmentFactor = adjustmentFactor;
            _currentQuota = minimumQuota;
        }

        public bool CheckQuota()
        {
            lock (_lock)
            {
                // Implementation of adaptive quota checking
                return true; // Placeholder
            }
        }

        public void AdjustQuota(bool success)
        {
            lock (_lock)
            {
                if (success)
                {
                    _currentQuota = Math.Min(_maximumQuota, 
                        (int)(_currentQuota * (1 + _adjustmentFactor)));
                }
                else
                {
                    _currentQuota = Math.Max(_minimumQuota, 
                        (int)(_currentQuota * (1 - _adjustmentFactor)));
                }
            }
        }
    }

    internal class BurstQuota
    {
        private readonly int _burstCapacity;
        private readonly TimeSpan _burstDuration;
        private readonly Queue<DateTime> _burstOperations;
        private readonly object _lock = new object();

        public BurstQuota(int burstCapacity, TimeSpan burstDuration)
        {
            _burstCapacity = burstCapacity;
            _burstDuration = burstDuration;
            _burstOperations = new Queue<DateTime>();
        }

        public bool CheckBurst()
        {
            lock (_lock)
            {
                CleanupExpiredBursts();
                return _burstOperations.Count < _burstCapacity;
            }
        }

        public void RecordBurst()
        {
            lock (_lock)
            {
                CleanupExpiredBursts();
                _burstOperations.Enqueue(DateTime.UtcNow);
            }
        }

        private void CleanupExpiredBursts()
        {
            var threshold = DateTime.UtcNow - _burstDuration;
            while (_burstOperations.Count > 0 && _burstOperations.Peek() < threshold)
            {
                _burstOperations.Dequeue();
            }
        }
    }

    internal class CostTracker
    {
        private readonly decimal _dailyBudget;
        private readonly Dictionary<string, decimal> _operationCosts;
        private decimal _currentDailyCost;
        private DateTime _lastReset;
        private readonly object _lock = new object();

        public CostTracker(decimal dailyBudget, Dictionary<string, decimal> operationCosts)
        {
            _dailyBudget = dailyBudget;
            _operationCosts = operationCosts;
            _currentDailyCost = 0;
            _lastReset = DateTime.UtcNow.Date;
        }

        public async Task<bool> CheckOperationCostAsync(string operation)
        {
            lock (_lock)
            {
                ResetIfNewDay();

                if (!_operationCosts.TryGetValue(operation, out var cost))
                {
                    cost = 1.0M; // Default cost
                }

                return (_currentDailyCost + cost) <= _dailyBudget;
            }
        }

        public void RecordOperationCost(string operation)
        {
            lock (_lock)
            {
                ResetIfNewDay();

                if (_operationCosts.TryGetValue(operation, out var cost))
                {
                    _currentDailyCost += cost;
                }
            }
        }

        private void ResetIfNewDay()
        {
            var today = DateTime.UtcNow.Date;
            if (today > _lastReset)
            {
                _currentDailyCost = 0;
                _lastReset = today;
            }
        }
    }
}
