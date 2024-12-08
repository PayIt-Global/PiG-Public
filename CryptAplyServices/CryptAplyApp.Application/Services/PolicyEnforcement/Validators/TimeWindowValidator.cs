using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services.PolicyEnforcement.Validators
{
    public class TimeWindowValidator : ITimeWindowValidator
    {
        private readonly ILogger<TimeWindowValidator> _logger;
        private readonly IAuditLogger _auditLogger;

        public TimeWindowValidator(ILogger<TimeWindowValidator> logger, IAuditLogger auditLogger)
        {
            _logger = logger;
            _auditLogger = auditLogger;
        }

        public async Task<bool> IsWithinAllowedWindowAsync(KeyOperationContext context, TimeWindowControls controls)
        {
            try
            {
                var currentTime = context.Timestamp;
                var currentTimeZone = TimeZoneInfo.FindSystemTimeZoneById(controls.OperatingTimeZone.Id);
                var localTime = TimeZoneInfo.ConvertTime(currentTime, currentTimeZone);

                // Check if current day is allowed
                if (!controls.AllowedDays.Contains(localTime.DayOfWeek.ToString()))
                {
                    _logger.LogWarning("Operation attempted outside allowed days for {ApplicationId}", context.ApplicationId);
                    return false;
                }

                // Check if within allowed time windows
                var isWithinWindow = controls.AllowedWindows.Any(window =>
                {
                    var startTime = localTime.Date.Add(window.Start);
                    var endTime = localTime.Date.Add(window.End);
                    return localTime >= startTime && localTime <= endTime;
                });

                if (!isWithinWindow)
                {
                    _logger.LogWarning("Operation attempted outside allowed time windows for {ApplicationId}", 
                        context.ApplicationId);
                    return false;
                }

                // Check business hours if required
                if (controls.EnforceBusinessHours)
                {
                    var isBusinessHours = IsWithinBusinessHours(localTime);
                    if (!isBusinessHours)
                    {
                        _logger.LogWarning("Operation attempted outside business hours for {ApplicationId}", 
                            context.ApplicationId);
                        return false;
                    }
                }

                await _auditLogger.LogAsync(new AuditEvent
                {
                    EventType = "TimeWindowValidation",
                    ApplicationId = context.ApplicationId,
                    Success = true,
                    Timestamp = DateTime.UtcNow,
                    Metadata = new Dictionary<string, string>
                    {
                        { "LocalTime", localTime.ToString() },
                        { "TimeZone", controls.OperatingTimeZone.Id }
                    }
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating time window for {ApplicationId}", context.ApplicationId);
                return false;
            }
        }

        public async Task<bool> ValidateEmergencyWindowAsync(KeyOperationContext context, EmergencyAccess controls)
        {
            if (!controls.Allowed)
            {
                _logger.LogWarning("Emergency access not allowed for {ApplicationId}", context.ApplicationId);
                return false;
            }

            try
            {
                // Verify emergency approvers
                var hasRequiredApprovers = context is KeyRotationContext rotationContext &&
                                         rotationContext.EmergencyApprovers != null &&
                                         rotationContext.EmergencyApprovers.Length >= controls.RequiredApprovers &&
                                         rotationContext.EmergencyApprovers.All(approver =>
                                             controls.EmergencyApproverRoles.Contains(approver));

                if (!hasRequiredApprovers)
                {
                    _logger.LogWarning("Insufficient emergency approvers for {ApplicationId}", context.ApplicationId);
                    return false;
                }

                await _auditLogger.LogAsync(new AuditEvent
                {
                    EventType = "EmergencyAccess",
                    ApplicationId = context.ApplicationId,
                    Success = true,
                    Timestamp = DateTime.UtcNow,
                    Details = context is KeyRotationContext rotContext ?
                        rotContext.EmergencyApprovers :
                        Array.Empty<string>()
                });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating emergency window for {ApplicationId}", context.ApplicationId);
                return false;
            }
        }

        private bool IsWithinBusinessHours(DateTime localTime)
        {
            // Default business hours: Monday-Friday, 9 AM to 5 PM
            if (localTime.DayOfWeek == DayOfWeek.Saturday || localTime.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            var businessStart = new TimeSpan(9, 0, 0);
            var businessEnd = new TimeSpan(17, 0, 0);
            var currentTime = localTime.TimeOfDay;

            return currentTime >= businessStart && currentTime <= businessEnd;
        }
    }
}
