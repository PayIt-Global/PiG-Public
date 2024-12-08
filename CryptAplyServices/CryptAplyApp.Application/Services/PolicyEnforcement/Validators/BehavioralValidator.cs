using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.PolicyEnforcement.Validators
{
    public class BehavioralValidator : IBehavioralValidator
    {
        private readonly ILogger<BehavioralValidator> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly IAlertingService _alertingService;
        private readonly Dictionary<string, ApplicationBehaviorProfile> _behaviorProfiles;
        private readonly object _lockObject = new object();

        public BehavioralValidator(
            ILogger<BehavioralValidator> logger,
            IAuditLogger auditLogger,
            IAlertingService alertingService)
        {
            _logger = logger;
            _auditLogger = auditLogger;
            _alertingService = alertingService;
            _behaviorProfiles = new Dictionary<string, ApplicationBehaviorProfile>();
        }

        public async Task<bool> ValidateBehaviorAsync(KeyOperationContext context)
        {
            try
            {
                var profile = GetOrCreateProfile(context.ApplicationId);
                var anomalyScore = await AnalyzeBehaviorAsync(context, profile);

                await _auditLogger.LogAsync(new AuditEvent
                {
                    EventType = "BehavioralAnalysis",
                    ApplicationId = context.ApplicationId,
                    Success = anomalyScore < 0.8,
                    Timestamp = DateTime.UtcNow,
                    Details = new[] { $"Anomaly Score: {anomalyScore}" },
                    Metadata = new Dictionary<string, string>
                    {
                        { "AnomalyScore", anomalyScore.ToString("F2") },
                        { "Operation", context.Operation }
                    }
                });

                if (anomalyScore >= 0.8)
                {
                    await _alertingService.RaiseAlertAsync(new Alert
                    {
                        Severity = AlertSeverity.High,
                        Source = "BehavioralValidator",
                        Message = $"Anomalous behavior detected for {context.ApplicationId}",
                        Timestamp = DateTime.UtcNow,
                        Details = $"Anomaly Score: {anomalyScore}"
                    });
                    return false;
                }

                // Update behavioral profile
                profile.UpdateProfile(context);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating behavior for {ApplicationId}", context.ApplicationId);
                return false;
            }
        }

        private ApplicationBehaviorProfile GetOrCreateProfile(string applicationId)
        {
            lock (_lockObject)
            {
                if (!_behaviorProfiles.TryGetValue(applicationId, out var profile))
                {
                    profile = new ApplicationBehaviorProfile(applicationId);
                    _behaviorProfiles[applicationId] = profile;
                }
                return profile;
            }
        }

        private async Task<double> AnalyzeBehaviorAsync(
            KeyOperationContext context,
            ApplicationBehaviorProfile profile)
        {
            var anomalyScore = 0.0;

            // Check operation frequency pattern
            anomalyScore += AnalyzeOperationFrequency(context, profile);

            // Check time pattern
            anomalyScore += AnalyzeTimePattern(context, profile);

            // Check operation sequence pattern
            anomalyScore += AnalyzeOperationSequence(context, profile);

            // Check data volume pattern
            anomalyScore += await AnalyzeDataVolumePatternAsync(context, profile);

            // Normalize final score
            return Math.Min(1.0, anomalyScore / 4.0);
        }

        private double AnalyzeOperationFrequency(
            KeyOperationContext context,
            ApplicationBehaviorProfile profile)
        {
            var recentOperations = profile.GetRecentOperations(TimeSpan.FromMinutes(5));
            var frequency = recentOperations.Count();
            
            // Compare with historical average
            var historicalAvg = profile.GetAverageOperationFrequency();
            if (historicalAvg == 0) return 0.0;

            var deviation = Math.Abs(frequency - historicalAvg) / historicalAvg;
            return Math.Min(1.0, deviation);
        }

        private double AnalyzeTimePattern(
            KeyOperationContext context,
            ApplicationBehaviorProfile profile)
        {
            var currentHour = DateTime.UtcNow.Hour;
            var historicalActivity = profile.GetHistoricalActivityByHour(currentHour);
            
            if (historicalActivity.Count == 0) return 0.5;

            var avgActivity = historicalActivity.Average();
            var stdDev = CalculateStandardDeviation(historicalActivity, avgActivity);
            
            var currentActivity = profile.GetRecentOperations(TimeSpan.FromHours(1)).Count();
            var zScore = Math.Abs(currentActivity - avgActivity) / (stdDev == 0 ? 1 : stdDev);
            
            return Math.Min(1.0, zScore / 3.0);
        }

        private double AnalyzeOperationSequence(
            KeyOperationContext context,
            ApplicationBehaviorProfile profile)
        {
            var recentSequence = profile.GetRecentOperations(TimeSpan.FromMinutes(10))
                                      .Select(op => op.Operation)
                                      .ToList();

            if (recentSequence.Count < 3) return 0.0;

            var sequenceScore = profile.CompareWithHistoricalSequences(recentSequence);
            return sequenceScore;
        }

        private async Task<double> AnalyzeDataVolumePatternAsync(
            KeyOperationContext context,
            ApplicationBehaviorProfile profile)
        {
            var recentVolume = await profile.GetRecentDataVolumeAsync(TimeSpan.FromHours(1));
            var historicalVolume = profile.GetAverageHourlyDataVolume();
            
            if (historicalVolume == 0) return 0.0;

            var deviation = Math.Abs(recentVolume - historicalVolume) / historicalVolume;
            return Math.Min(1.0, deviation);
        }

        private double CalculateStandardDeviation(IEnumerable<double> values, double mean)
        {
            return Math.Sqrt(values.Select(v => Math.Pow(v - mean, 2)).Average());
        }
    }

    internal class ApplicationBehaviorProfile
    {
        private readonly string _applicationId;
        private readonly Queue<OperationRecord> _recentOperations;
        private readonly Dictionary<int, List<double>> _hourlyActivityHistory;
        private readonly List<List<string>> _operationSequences;
        private double _totalDataVolume;
        private int _totalHours;
        private readonly object _lockObject = new object();

        public ApplicationBehaviorProfile(string applicationId)
        {
            _applicationId = applicationId;
            _recentOperations = new Queue<OperationRecord>();
            _hourlyActivityHistory = new Dictionary<int, List<double>>();
            _operationSequences = new List<List<string>>();
            _totalDataVolume = 0;
            _totalHours = 0;
        }

        public void UpdateProfile(KeyOperationContext context)
        {
            lock (_lockObject)
            {
                // Add new operation record
                var record = new OperationRecord
                {
                    Timestamp = DateTime.UtcNow,
                    Operation = context.Operation,
                    DataVolume = EstimateDataVolume(context)
                };

                _recentOperations.Enqueue(record);

                // Update hourly activity
                var hour = record.Timestamp.Hour;
                if (!_hourlyActivityHistory.ContainsKey(hour))
                {
                    _hourlyActivityHistory[hour] = new List<double>();
                }
                _hourlyActivityHistory[hour].Add(1.0);

                // Update operation sequences
                var recentOps = GetRecentOperations(TimeSpan.FromMinutes(10))
                    .Select(op => op.Operation)
                    .ToList();
                if (recentOps.Count >= 3)
                {
                    _operationSequences.Add(recentOps);
                }

                // Update data volume metrics
                _totalDataVolume += record.DataVolume;
                _totalHours++;

                // Cleanup old records
                CleanupOldRecords();
            }
        }

        public IEnumerable<OperationRecord> GetRecentOperations(TimeSpan window)
        {
            var cutoff = DateTime.UtcNow - window;
            return _recentOperations.Where(r => r.Timestamp >= cutoff);
        }

        public double GetAverageOperationFrequency()
        {
            return _recentOperations.Count > 0 ? 
                _recentOperations.Count / (DateTime.UtcNow - _recentOperations.First().Timestamp).TotalHours : 
                0;
        }

        public List<double> GetHistoricalActivityByHour(int hour)
        {
            return _hourlyActivityHistory.TryGetValue(hour, out var activity) ? 
                activity : new List<double>();
        }

        public double CompareWithHistoricalSequences(List<string> currentSequence)
        {
            if (_operationSequences.Count == 0) return 0.5;

            var similarities = _operationSequences
                .Select(seq => CalculateSequenceSimilarity(seq, currentSequence))
                .OrderByDescending(s => s)
                .Take(5)
                .Average();

            return 1.0 - similarities;
        }

        public Task<double> GetRecentDataVolumeAsync(TimeSpan window)
        {
            var recentOps = GetRecentOperations(window);
            return Task.FromResult(recentOps.Sum(op => op.DataVolume));
        }

        public double GetAverageHourlyDataVolume()
        {
            return _totalHours > 0 ? _totalDataVolume / _totalHours : 0;
        }

        private void CleanupOldRecords()
        {
            var cutoff = DateTime.UtcNow - TimeSpan.FromDays(7);
            while (_recentOperations.Count > 0 && _recentOperations.Peek().Timestamp < cutoff)
            {
                _recentOperations.Dequeue();
            }

            if (_operationSequences.Count > 1000)
            {
                _operationSequences.RemoveRange(0, 500);
            }
        }

        private double CalculateSequenceSimilarity(List<string> seq1, List<string> seq2)
        {
            var length = Math.Min(seq1.Count, seq2.Count);
            var matches = 0;

            for (int i = 0; i < length; i++)
            {
                if (seq1[i] == seq2[i]) matches++;
            }

            return (double)matches / length;
        }

        private double EstimateDataVolume(KeyOperationContext context)
        {
            // Implementation would estimate data volume based on operation type
            return 1.0; // Placeholder implementation
        }
    }

    internal class OperationRecord
    {
        public DateTime Timestamp { get; set; }
        public string Operation { get; set; }
        public double DataVolume { get; set; }
    }
}
