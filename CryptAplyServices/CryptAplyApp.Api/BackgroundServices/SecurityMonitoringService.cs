using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Services;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Api.BackgroundServices
{
    public class SecurityMonitoringService : BackgroundService
    {
        private readonly ILogger<SecurityMonitoringService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public SecurityMonitoringService(
            ILogger<SecurityMonitoringService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var auditLoggingService = scope.ServiceProvider.GetRequiredService<AuditLoggingService>();
                        var alertingService = scope.ServiceProvider.GetRequiredService<AlertingService>();
                        var complianceMonitoringService = scope.ServiceProvider.GetRequiredService<ComplianceMonitoringService>();

                        // Check for suspicious activities
                        var suspiciousActivities = await auditLoggingService.GetSuspiciousActivitiesAsync(
                            DateTime.UtcNow.AddHours(-1), // Look at the last hour
                            DateTime.UtcNow);

                        foreach (var activity in suspiciousActivities)
                        {
                            // Create high-priority alert for suspicious activity
                            await alertingService.CreateAlertAsync(
                                $"Suspicious Activity Detected - {activity.UserId}",
                                $"Suspicious activity detected: {activity.Details}",
                                AlertSeverity.High,
                                "Security",
                                "System");

                            // Log the detection
                            await auditLoggingService.LogActivityAsync(new AuditLogEntry
                            {
                                UserId = "System",
                                Action = "SuspiciousActivityDetected",
                                ResourceId = activity.Id,
                                ResourceType = "SecurityAlert",
                                Timestamp = DateTime.UtcNow,
                                Sensitivity = ActivitySensitivity.High,
                                Details = $"Suspicious activity detected: {activity.Details}"
                            });
                        }

                        // Check compliance status
                        var complianceIssues = await complianceMonitoringService.CheckComplianceStatusAsync();
                        foreach (var issue in complianceIssues)
                        {
                            // Create alert for compliance issues
                            await alertingService.CreateAlertAsync(
                                $"Compliance Issue Detected - {issue.Category}",
                                issue.Description,
                                AlertSeverity.High,
                                "Compliance",
                                "System");
                        }

                        // Check key expiration
                        var expiringKeys = await complianceMonitoringService.CheckKeyExpirationAsync();
                        foreach (var key in expiringKeys)
                        {
                            await alertingService.CreateAlertAsync(
                                $"Key Expiring Soon - {key.Id}",
                                $"Encryption key {key.Id} will expire in {key.DaysUntilExpiration} days",
                                AlertSeverity.Medium,
                                "KeyManagement",
                                "System");
                        }
                    }

                    // Wait for 5 minutes before next check
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in security monitoring service");
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait 1 minute before retrying on error
                }
            }
        }
    }
}
