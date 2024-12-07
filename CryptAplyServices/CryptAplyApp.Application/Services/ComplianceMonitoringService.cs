using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services
{
    public interface IComplianceMonitoringService
    {
        Task MonitorVendorRiskScoresAsync();
        Task MonitorSecurityIncidentsAsync();
        Task MonitorComplianceDeadlinesAsync();
        Task MonitorKeyRotationScheduleAsync();
        Task MonitorAuditLogActivitiesAsync();
        Task<List<ComplianceMetric>> GetComplianceMetricsAsync();
    }

    public class ComplianceMonitoringService : IComplianceMonitoringService
    {
        private readonly ILogger<ComplianceMonitoringService> _logger;
        private readonly IVendorRiskManagementService _vendorService;
        private readonly IIncidentResponseService _incidentService;
        private readonly IComplianceCalendarService _calendarService;
        private readonly IKeyManagementService _keyService;
        private readonly IAlertingService _alertingService;
        private readonly IMetricsService _metricsService;

        public ComplianceMonitoringService(
            ILogger<ComplianceMonitoringService> logger,
            IVendorRiskManagementService vendorService,
            IIncidentResponseService incidentService,
            IComplianceCalendarService calendarService,
            IKeyManagementService keyService,
            IAlertingService alertingService,
            IMetricsService metricsService)
        {
            _logger = logger;
            _vendorService = vendorService;
            _incidentService = incidentService;
            _calendarService = calendarService;
            _keyService = keyService;
            _alertingService = alertingService;
            _metricsService = metricsService;
        }

        public async Task MonitorVendorRiskScoresAsync()
        {
            try
            {
                _logger.LogInformation("Starting vendor risk score monitoring");

                // Get all vendors and their risk scores
                var vendors = await _vendorService.GetAllVendorsAsync();
                foreach (var vendor in vendors)
                {
                    var riskScore = vendor.RiskScore;
                    if (riskScore == null)
                        continue;

                    // Monitor overall risk score
                    if (riskScore.OverallScore >= 7.0)
                    {
                        await _alertingService.SendAlertAsync(new Alert
                        {
                            Type = AlertType.HighRisk,
                            Severity = AlertSeverity.High,
                            Title = $"High Risk Score for Vendor {vendor.Name}",
                            Description = $"Vendor risk score is {riskScore.OverallScore:F1}, which exceeds the threshold of 7.0",
                            Category = "VendorRisk",
                            Tags = new[] { "Vendor", "Risk", "Compliance" },
                            Metadata = new Dictionary<string, string>
                            {
                                { "VendorId", vendor.VendorId },
                                { "VendorName", vendor.Name },
                                { "RiskScore", riskScore.OverallScore.ToString("F1") }
                            }
                        });
                    }

                    // Monitor risk trend
                    if (riskScore.Trend == RiskTrend.Worsening)
                    {
                        await _alertingService.SendAlertAsync(new Alert
                        {
                            Type = AlertType.RiskTrend,
                            Severity = AlertSeverity.Medium,
                            Title = $"Worsening Risk Trend for Vendor {vendor.Name}",
                            Description = "Vendor's risk score is showing a worsening trend",
                            Category = "VendorRisk",
                            Tags = new[] { "Vendor", "Risk", "Trend" },
                            Metadata = new Dictionary<string, string>
                            {
                                { "VendorId", vendor.VendorId },
                                { "VendorName", vendor.Name },
                                { "RiskTrend", riskScore.Trend.ToString() }
                            }
                        });
                    }

                    // Record metrics
                    await _metricsService.RecordMetricAsync("vendor_risk_score", riskScore.OverallScore,
                        new Dictionary<string, string>
                        {
                            { "vendor_id", vendor.VendorId },
                            { "vendor_name", vendor.Name }
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error monitoring vendor risk scores");
                throw;
            }
        }

        public async Task MonitorSecurityIncidentsAsync()
        {
            try
            {
                _logger.LogInformation("Starting security incident monitoring");

                var incidents = await _incidentService.GetAllIncidentsAsync();
                var activeIncidents = incidents.Where(i => i.Status != IncidentStatus.Resolved && i.Status != IncidentStatus.Closed);

                foreach (var incident in activeIncidents)
                {
                    // Monitor critical incidents
                    if (incident.Severity == IncidentSeverity.Critical)
                    {
                        var timeOpen = DateTime.UtcNow - incident.DetectionTime;
                        if (timeOpen.TotalHours >= 4) // Alert if critical incident is open for more than 4 hours
                        {
                            await _alertingService.SendAlertAsync(new Alert
                            {
                                Type = AlertType.CriticalIncident,
                                Severity = AlertSeverity.Critical,
                                Title = $"Critical Incident {incident.IncidentId} Open for {timeOpen.TotalHours:F1} Hours",
                                Description = $"Critical security incident '{incident.Title}' has been open for more than 4 hours",
                                Category = "SecurityIncident",
                                Tags = new[] { "Security", "Incident", "Critical" },
                                Metadata = new Dictionary<string, string>
                                {
                                    { "IncidentId", incident.IncidentId },
                                    { "Title", incident.Title },
                                    { "TimeOpen", timeOpen.ToString() }
                                }
                            });
                        }
                    }

                    // Monitor containment actions
                    var pendingActions = incident.ContainmentActions.Count(a => a.Status == ActionStatus.Pending);
                    if (pendingActions > 0)
                    {
                        await _alertingService.SendAlertAsync(new Alert
                        {
                            Type = AlertType.PendingActions,
                            Severity = AlertSeverity.Medium,
                            Title = $"Pending Containment Actions for Incident {incident.IncidentId}",
                            Description = $"{pendingActions} containment actions are pending for incident '{incident.Title}'",
                            Category = "SecurityIncident",
                            Tags = new[] { "Security", "Incident", "Containment" },
                            Metadata = new Dictionary<string, string>
                            {
                                { "IncidentId", incident.IncidentId },
                                { "Title", incident.Title },
                                { "PendingActions", pendingActions.ToString() }
                            }
                        });
                    }

                    // Record metrics
                    await _metricsService.RecordMetricAsync("incident_time_to_resolution",
                        (incident.ResolutionTime - incident.DetectionTime)?.TotalHours ?? 0,
                        new Dictionary<string, string>
                        {
                            { "incident_id", incident.IncidentId },
                            { "severity", incident.Severity.ToString() }
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error monitoring security incidents");
                throw;
            }
        }

        public async Task MonitorComplianceDeadlinesAsync()
        {
            try
            {
                _logger.LogInformation("Starting compliance deadline monitoring");

                var calendar = await _calendarService.GetCalendarAsync(DateTime.UtcNow.Year);
                if (calendar == null)
                    return;

                foreach (var deadline in calendar.Deadlines)
                {
                    var timeToDeadline = deadline.DueDate - DateTime.UtcNow;

                    // Alert for approaching deadlines
                    if (timeToDeadline.TotalDays <= 7 && deadline.Status == DeadlineStatus.Upcoming)
                    {
                        await _alertingService.SendAlertAsync(new Alert
                        {
                            Type = AlertType.DeadlineApproaching,
                            Severity = AlertSeverity.Medium,
                            Title = $"Compliance Deadline Approaching: {deadline.Title}",
                            Description = $"Compliance deadline '{deadline.Title}' is due in {timeToDeadline.TotalDays:F1} days",
                            Category = "ComplianceDeadline",
                            Tags = new[] { "Compliance", "Deadline", "Upcoming" },
                            Metadata = new Dictionary<string, string>
                            {
                                { "DeadlineId", deadline.DeadlineId },
                                { "Title", deadline.Title },
                                { "DueDate", deadline.DueDate.ToString("yyyy-MM-dd") },
                                { "DaysRemaining", timeToDeadline.TotalDays.ToString("F1") }
                            }
                        });
                    }

                    // Alert for missed deadlines
                    if (deadline.Status == DeadlineStatus.Missed)
                    {
                        await _alertingService.SendAlertAsync(new Alert
                        {
                            Type = AlertType.DeadlineMissed,
                            Severity = AlertSeverity.High,
                            Title = $"Compliance Deadline Missed: {deadline.Title}",
                            Description = $"Compliance deadline '{deadline.Title}' was due on {deadline.DueDate:yyyy-MM-dd} and has been missed",
                            Category = "ComplianceDeadline",
                            Tags = new[] { "Compliance", "Deadline", "Missed" },
                            Metadata = new Dictionary<string, string>
                            {
                                { "DeadlineId", deadline.DeadlineId },
                                { "Title", deadline.Title },
                                { "DueDate", deadline.DueDate.ToString("yyyy-MM-dd") }
                            }
                        });
                    }

                    // Record metrics
                    await _metricsService.RecordMetricAsync("compliance_deadline_status",
                        (int)deadline.Status,
                        new Dictionary<string, string>
                        {
                            { "deadline_id", deadline.DeadlineId },
                            { "title", deadline.Title }
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error monitoring compliance deadlines");
                throw;
            }
        }

        public async Task MonitorKeyRotationScheduleAsync()
        {
            try
            {
                _logger.LogInformation("Starting key rotation schedule monitoring");

                var calendar = await _calendarService.GetCalendarAsync(DateTime.UtcNow.Year);
                if (calendar == null)
                    return;

                var keyRotationEvents = calendar.Events
                    .Where(e => e.Type == EventType.KeyRotation && e.StartDate >= DateTime.UtcNow)
                    .OrderBy(e => e.StartDate);

                foreach (var rotationEvent in keyRotationEvents)
                {
                    var timeToRotation = rotationEvent.StartDate - DateTime.UtcNow;

                    // Alert for upcoming key rotations
                    if (timeToRotation.TotalDays <= 14)
                    {
                        await _alertingService.SendAlertAsync(new Alert
                        {
                            Type = AlertType.KeyRotation,
                            Severity = AlertSeverity.Medium,
                            Title = $"Key Rotation Scheduled: {rotationEvent.Title}",
                            Description = $"Key rotation event '{rotationEvent.Title}' is scheduled in {timeToRotation.TotalDays:F1} days",
                            Category = "KeyManagement",
                            Tags = new[] { "Key", "Rotation", "Scheduled" },
                            Metadata = new Dictionary<string, string>
                            {
                                { "EventId", rotationEvent.EventId },
                                { "Title", rotationEvent.Title },
                                { "ScheduledDate", rotationEvent.StartDate.ToString("yyyy-MM-dd") }
                            }
                        });
                    }

                    // Record metrics
                    await _metricsService.RecordMetricAsync("key_rotation_schedule",
                        timeToRotation.TotalDays,
                        new Dictionary<string, string>
                        {
                            { "event_id", rotationEvent.EventId },
                            { "title", rotationEvent.Title }
                        });
                }

                // Monitor key usage and age
                var keys = await _keyService.GetAllKeysAsync();
                foreach (var key in keys)
                {
                    var keyAge = DateTime.UtcNow - key.CreationDate;

                    // Alert for keys approaching maximum age
                    if (keyAge.TotalDays >= 330) // Alert 35 days before 1-year maximum
                    {
                        await _alertingService.SendAlertAsync(new Alert
                        {
                            Type = AlertType.KeyAge,
                            Severity = AlertSeverity.High,
                            Title = $"Key Approaching Maximum Age: {key.KeyId}",
                            Description = $"Cryptographic key is {keyAge.TotalDays:F1} days old and approaching maximum age of 365 days",
                            Category = "KeyManagement",
                            Tags = new[] { "Key", "Age", "Rotation" },
                            Metadata = new Dictionary<string, string>
                            {
                                { "KeyId", key.KeyId },
                                { "KeyAge", keyAge.TotalDays.ToString("F1") }
                            }
                        });
                    }

                    // Record metrics
                    await _metricsService.RecordMetricAsync("key_age",
                        keyAge.TotalDays,
                        new Dictionary<string, string>
                        {
                            { "key_id", key.KeyId },
                            { "key_type", key.Type.ToString() }
                        });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error monitoring key rotation schedule");
                throw;
            }
        }

        public async Task MonitorAuditLogActivitiesAsync()
        {
            try
            {
                _logger.LogInformation("Starting audit log activity monitoring");

                var startTime = DateTime.UtcNow.AddHours(-1);
                var auditLogs = await _keyService.GetAuditLogsAsync(startTime, DateTime.UtcNow);

                // Monitor for suspicious activities
                var suspiciousActivities = auditLogs
                    .Where(log => log.ActivityType == ActivityType.KeyAccess || log.ActivityType == ActivityType.KeyModification)
                    .GroupBy(log => log.UserId)
                    .Where(group => group.Count() > 10); // More than 10 key operations in an hour

                foreach (var activity in suspiciousActivities)
                {
                    await _alertingService.SendAlertAsync(new Alert
                    {
                        Type = AlertType.SuspiciousActivity,
                        Severity = AlertSeverity.High,
                        Title = $"Suspicious Key Activity Detected",
                        Description = $"User {activity.Key} performed {activity.Count()} key operations in the last hour",
                        Category = "Security",
                        Tags = new[] { "Audit", "Security", "Suspicious" },
                        Metadata = new Dictionary<string, string>
                        {
                            { "UserId", activity.Key },
                            { "ActivityCount", activity.Count().ToString() },
                            { "TimeWindow", "1 hour" }
                        }
                    });
                }

                // Monitor failed operations
                var failedOperations = auditLogs.Where(log => log.Status == OperationStatus.Failed);
                foreach (var operation in failedOperations)
                {
                    await _alertingService.SendAlertAsync(new Alert
                    {
                        Type = AlertType.FailedOperation,
                        Severity = AlertSeverity.Medium,
                        Title = $"Failed Key Operation: {operation.ActivityType}",
                        Description = $"Key operation failed for user {operation.UserId}: {operation.ErrorMessage}",
                        Category = "Security",
                        Tags = new[] { "Audit", "Failed", operation.ActivityType.ToString() },
                        Metadata = new Dictionary<string, string>
                        {
                            { "UserId", operation.UserId },
                            { "ActivityType", operation.ActivityType.ToString() },
                            { "ErrorMessage", operation.ErrorMessage }
                        }
                    });
                }

                // Record metrics
                await _metricsService.RecordMetricAsync("audit_log_activities",
                    auditLogs.Count(),
                    new Dictionary<string, string>
                    {
                        { "time_window", "1h" }
                    });

                await _metricsService.RecordMetricAsync("failed_operations",
                    failedOperations.Count(),
                    new Dictionary<string, string>
                    {
                        { "time_window", "1h" }
                    });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error monitoring audit log activities");
                throw;
            }
        }

        public async Task<List<ComplianceMetric>> GetComplianceMetricsAsync()
        {
            var metrics = new List<ComplianceMetric>();

            try
            {
                // Vendor Risk Metrics
                var vendors = await _vendorService.GetAllVendorsAsync();
                metrics.Add(new ComplianceMetric
                {
                    Name = "average_vendor_risk_score",
                    Value = vendors.Average(v => v.RiskScore?.OverallScore ?? 0),
                    Labels = new Dictionary<string, string>
                    {
                        { "metric_type", "vendor_risk" }
                    }
                });

                metrics.Add(new ComplianceMetric
                {
                    Name = "high_risk_vendors",
                    Value = vendors.Count(v => v.RiskScore?.OverallScore >= 7.0),
                    Labels = new Dictionary<string, string>
                    {
                        { "metric_type", "vendor_risk" },
                        { "risk_level", "high" }
                    }
                });

                // Incident Metrics
                var incidents = await _incidentService.GetAllIncidentsAsync();
                var activeIncidents = incidents.Where(i => i.Status != IncidentStatus.Resolved && i.Status != IncidentStatus.Closed);

                metrics.Add(new ComplianceMetric
                {
                    Name = "active_incidents",
                    Value = activeIncidents.Count(),
                    Labels = new Dictionary<string, string>
                    {
                        { "metric_type", "security_incident" }
                    }
                });

                metrics.Add(new ComplianceMetric
                {
                    Name = "critical_incidents",
                    Value = activeIncidents.Count(i => i.Severity == IncidentSeverity.Critical),
                    Labels = new Dictionary<string, string>
                    {
                        { "metric_type", "security_incident" },
                        { "severity", "critical" }
                    }
                });

                // Compliance Deadline Metrics
                var calendar = await _calendarService.GetCalendarAsync(DateTime.UtcNow.Year);
                if (calendar != null)
                {
                    metrics.Add(new ComplianceMetric
                    {
                        Name = "upcoming_deadlines",
                        Value = calendar.Deadlines.Count(d => d.Status == DeadlineStatus.Upcoming),
                        Labels = new Dictionary<string, string>
                        {
                            { "metric_type", "compliance_deadline" }
                        }
                    });

                    metrics.Add(new ComplianceMetric
                    {
                        Name = "missed_deadlines",
                        Value = calendar.Deadlines.Count(d => d.Status == DeadlineStatus.Missed),
                        Labels = new Dictionary<string, string>
                        {
                            { "metric_type", "compliance_deadline" }
                        }
                    });
                }

                // Key Management Metrics
                var keys = await _keyService.GetAllKeysAsync();
                metrics.Add(new ComplianceMetric
                {
                    Name = "keys_near_rotation",
                    Value = keys.Count(k => (DateTime.UtcNow - k.CreationDate).TotalDays >= 330),
                    Labels = new Dictionary<string, string>
                    {
                        { "metric_type", "key_management" }
                    }
                });

                return metrics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting compliance metrics");
                throw;
            }
        }
    }
}
