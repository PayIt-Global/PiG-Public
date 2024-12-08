using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.PolicyEnforcement.Validators
{
    public class EnhancedTimeWindowValidator : ITimeWindowValidator
    {
        private readonly ILogger<EnhancedTimeWindowValidator> _logger;
        private readonly IAuditLogger _auditLogger;
        private readonly Dictionary<string, LoadMetrics> _loadMetrics;
        private readonly object _loadLock = new object();

        public EnhancedTimeWindowValidator(
            ILogger<EnhancedTimeWindowValidator> logger,
            IAuditLogger auditLogger)
        {
            _logger = logger;
            _auditLogger = auditLogger;
            _loadMetrics = new Dictionary<string, LoadMetrics>();
        }

        public async Task<bool> IsWithinAllowedWindowAsync(KeyOperationContext context, TimeWindowControls controls)
        {
            try
            {
                var enhancedControls = controls as EnhancedTimeWindowControls;
                if (enhancedControls == null)
                {
                    return await base.IsWithinAllowedWindowAsync(context, controls);
                }

                var currentTime = context.Timestamp;
                var currentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(controls.OperatingTimeZone.Id);
                var localTime = TimeZoneInfo.ConvertTime(currentTime, currentTimeZone);

                // Check holidays
                if (!enhancedControls.AllowOperationsDuringHolidays && 
                    IsHoliday(localTime, enhancedControls.Holidays))
                {
                    await LogValidationResultAsync(context, "Operation attempted during holiday", false);
                    return false;
                }

                // Check maintenance windows
                if (enhancedControls.BlockDuringMaintenance && 
                    IsInMaintenanceWindow(localTime, enhancedControls.PlannedMaintenance))
                {
                    var maintenanceWindow = GetCurrentMaintenanceWindow(localTime, enhancedControls.PlannedMaintenance);
                    if (!maintenanceWindow.AllowEmergencyOperations || !context.IsEmergencyOperation)
                    {
                        await LogValidationResultAsync(context, 
                            $"Operation attempted during maintenance window: {maintenanceWindow.Description}", false);
                        return false;
                    }
                }

                // Check region-specific windows
                if (enhancedControls.EnforceRegionalRestrictions &&
                    !await ValidateRegionalWindowAsync(context, enhancedControls.RegionSpecificWindows))
                {
                    return false;
                }

                // Check load-based throttling
                if (enhancedControls.EnableAutomaticLoadBalancing &&
                    !await ValidateLoadThrottlingAsync(context, enhancedControls.PeakHourThrottling))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating enhanced time window for {ApplicationId}", context.ApplicationId);
                return false;
            }
        }

        private bool IsHoliday(DateTime date, Dictionary<DateTime, string> holidays)
        {
            return holidays?.Any(h => h.Key.Date == date.Date) ?? false;
        }

        private bool IsInMaintenanceWindow(DateTime currentTime, List<MaintenanceWindow> maintenanceWindows)
        {
            return maintenanceWindows?.Any(window => 
                currentTime >= window.StartTime && currentTime <= window.EndTime) ?? false;
        }

        private MaintenanceWindow GetCurrentMaintenanceWindow(DateTime currentTime, List<MaintenanceWindow> maintenanceWindows)
        {
            return maintenanceWindows?.FirstOrDefault(window =>
                currentTime >= window.StartTime && currentTime <= window.EndTime);
        }

        private async Task<bool> ValidateRegionalWindowAsync(
            KeyOperationContext context,
            Dictionary<string, TimeWindowControls> regionWindows)
        {
            if (string.IsNullOrEmpty(context.Region) || !regionWindows.ContainsKey(context.Region))
            {
                await LogValidationResultAsync(context, 
                    $"No time window configuration found for region: {context.Region}", false);
                return false;
            }

            var regionControls = regionWindows[context.Region];
            return await IsWithinAllowedWindowAsync(context, regionControls);
        }

        private async Task<bool> ValidateLoadThrottlingAsync(
            KeyOperationContext context,
            Dictionary<TimeSpan, int> peakHourThrottling)
        {
            var currentLoad = GetCurrentLoad(context.Region);
            var currentHour = context.Timestamp.TimeOfDay;

            if (peakHourThrottling.TryGetValue(currentHour, out var threshold))
            {
                if (currentLoad >= threshold)
                {
                    await LogValidationResultAsync(context, 
                        $"Operation throttled due to peak hour load: {currentLoad}/{threshold}", false);
                    return false;
                }
            }

            return true;
        }

        private int GetCurrentLoad(string region)
        {
            lock (_loadLock)
            {
                if (!_loadMetrics.TryGetValue(region, out var metrics))
                {
                    metrics = new LoadMetrics();
                    _loadMetrics[region] = metrics;
                }

                metrics.CleanupOldOperations();
                return metrics.CurrentLoad;
            }
        }

        private async Task LogValidationResultAsync(
            KeyOperationContext context,
            string message,
            bool isSuccess)
        {
            await _auditLogger.LogAsync(new AuditEvent
            {
                EventType = "TimeWindowValidation",
                ApplicationId = context.ApplicationId,
                Success = isSuccess,
                Timestamp = DateTime.UtcNow,
                Details = new[] { message },
                Metadata = new Dictionary<string, string>
                {
                    { "Region", context.Region },
                    { "IsEmergencyOperation", context.IsEmergencyOperation.ToString() },
                    { "CurrentLoad", GetCurrentLoad(context.Region).ToString() }
                }
            });
        }
    }

    internal class LoadMetrics
    {
        private readonly Queue<DateTime> _operations;
        private readonly TimeSpan _metricWindow = TimeSpan.FromMinutes(5);

        public LoadMetrics()
        {
            _operations = new Queue<DateTime>();
        }

        public int CurrentLoad => _operations.Count;

        public void RecordOperation()
        {
            _operations.Enqueue(DateTime.UtcNow);
            CleanupOldOperations();
        }

        public void CleanupOldOperations()
        {
            var threshold = DateTime.UtcNow - _metricWindow;
            while (_operations.Count > 0 && _operations.Peek() < threshold)
            {
                _operations.Dequeue();
            }
        }
    }
}
