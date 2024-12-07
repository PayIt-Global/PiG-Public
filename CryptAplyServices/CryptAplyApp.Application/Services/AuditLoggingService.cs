using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Infrastructure.Repositories;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services
{
    public class AuditLoggingService
    {
        private readonly ILogger<AuditLoggingService> _logger;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly AlertingService _alertingService;

        public AuditLoggingService(
            ILogger<AuditLoggingService> logger,
            IAuditLogRepository auditLogRepository,
            AlertingService alertingService)
        {
            _logger = logger;
            _auditLogRepository = auditLogRepository;
            _alertingService = alertingService;
        }

        public async Task LogActivityAsync(AuditLogEntry entry)
        {
            try
            {
                // Set timestamp if not provided
                if (entry.Timestamp == default)
                {
                    entry.Timestamp = DateTime.UtcNow;
                }

                // Store the audit log
                await _auditLogRepository.CreateLogEntryAsync(entry);

                // Check for suspicious activities
                if (await IsSuspiciousActivityAsync(entry))
                {
                    await AlertSuspiciousActivityAsync(entry);
                }

                _logger.LogInformation(
                    "Audit log created: {Action} by {User} on {Resource}",
                    entry.Action,
                    entry.UserId,
                    entry.ResourceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit log entry");
                throw;
            }
        }

        public async Task<List<AuditLogEntry>> GetUserActivityAsync(string userId, DateTime startTime, DateTime endTime)
        {
            try
            {
                return await _auditLogRepository.GetUserActivityAsync(userId, startTime, endTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user activity for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<AuditLogEntry>> GetResourceActivityAsync(string resourceId, DateTime startTime, DateTime endTime)
        {
            try
            {
                return await _auditLogRepository.GetResourceActivityAsync(resourceId, startTime, endTime);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving activity for resource {ResourceId}", resourceId);
                throw;
            }
        }

        private async Task<bool> IsSuspiciousActivityAsync(AuditLogEntry entry)
        {
            try
            {
                // Get recent activity for this user
                var recentActivity = await _auditLogRepository.GetUserActivityAsync(
                    entry.UserId,
                    DateTime.UtcNow.AddMinutes(-5),
                    DateTime.UtcNow
                );

                // Check for rapid succession of sensitive operations
                if (entry.Sensitivity == ActivitySensitivity.High && recentActivity.Count > 5)
                {
                    return true;
                }

                // Check for operations from unusual locations
                if (entry.Location != null)
                {
                    var usualLocations = await _auditLogRepository.GetUserUsualLocationsAsync(entry.UserId);
                    if (!usualLocations.Contains(entry.Location))
                    {
                        return true;
                    }
                }

                // Check for operations outside normal hours
                var hour = entry.Timestamp.Hour;
                if (hour < 6 || hour > 22) // Assuming normal hours are 6 AM to 10 PM
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for suspicious activity");
                return false;
            }
        }

        private async Task AlertSuspiciousActivityAsync(AuditLogEntry entry)
        {
            await _alertingService.SendAlertAsync(new Alert
            {
                Title = "Suspicious Activity Detected",
                Description = $"Suspicious activity detected for user {entry.UserId}",
                Type = AlertType.SuspiciousActivity,
                Severity = AlertSeverity.High,
                Category = "Security",
                Tags = new List<string> { "security", "suspicious-activity" },
                Metadata = new Dictionary<string, string>
                {
                    { "UserId", entry.UserId },
                    { "Action", entry.Action },
                    { "ResourceId", entry.ResourceId },
                    { "Location", entry.Location ?? "Unknown" },
                    { "Timestamp", entry.Timestamp.ToString("yyyy-MM-dd HH:mm:ss UTC") }
                }
            });
        }
    }

    public class AuditLogEntry
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Action { get; set; }
        public string ResourceId { get; set; }
        public string ResourceType { get; set; }
        public DateTime Timestamp { get; set; }
        public string Location { get; set; }
        public string IpAddress { get; set; }
        public ActivitySensitivity Sensitivity { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public enum ActivitySensitivity
    {
        Low,
        Medium,
        High
    }
}
