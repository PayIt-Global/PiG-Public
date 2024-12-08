using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Services;
using CryptAplyApp.Application.Models;
using CryptAplyApp.Application.Interfaces;

namespace CryptAplyApp.Api.BackgroundServices
{
    public class KeyRotationBackgroundService : BackgroundService
    {
        private readonly ILogger<KeyRotationBackgroundService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly KeyRotationSchedulerOptions _options;

        public KeyRotationBackgroundService(
            ILogger<KeyRotationBackgroundService> logger,
            IServiceProvider serviceProvider,
            KeyRotationSchedulerOptions options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _options = options;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var keyRotationService = scope.ServiceProvider.GetRequiredService<KeyRotationService>();
                        var auditLoggingService = scope.ServiceProvider.GetRequiredService<AuditLoggingService>();
                        var alertingService = scope.ServiceProvider.GetRequiredService<AlertingService>();
                        var cryptoTeamService = scope.ServiceProvider.GetRequiredService<ICryptoTeamService>();
                        var notificationService = scope.ServiceProvider.GetRequiredService<NotificationServices>();

                        // Check for keys that need rotation
                        var keysToRotate = await keyRotationService.GetKeysNeedingRotationAsync();

                        foreach (var key in keysToRotate)
                        {
                            try
                            {
                                // Get the crypto team responsible for this key
                                var cryptoTeam = await cryptoTeamService.GetTeamForKeyAsync(key.Id);
                                if (cryptoTeam == null)
                                {
                                    _logger.LogError("No crypto team found for key {KeyId}", key.Id);
                                    continue;
                                }

                                // Check if a rotation request is already pending
                                var pendingRotation = await keyRotationService.GetPendingRotationRequestAsync(key.Id);
                                if (pendingRotation != null)
                                {
                                    // If the request is old and still doesn't have enough approvals, escalate
                                    if (pendingRotation.CreatedAt < DateTime.UtcNow.AddDays(-_options.EscalationThresholdDays))
                                    {
                                        await alertingService.CreateAlertAsync(
                                            $"Key Rotation Request Escalation - {key.Id}",
                                            $"Key rotation request for {key.Id} has been pending for {_options.EscalationThresholdDays} days without sufficient approvals.",
                                            AlertSeverity.High,
                                            "KeyRotation",
                                            "System");

                                        // Notify security officers
                                        await notificationService.NotifySecurityOfficersAsync(
                                            "Key Rotation Escalation",
                                            $"Key {key.Id} rotation request requires immediate attention.");
                                    }
                                    continue;
                                }

                                // Initiate key rotation request
                                var rotationRequest = await keyRotationService.InitiateKeyRotationAsync(key.Id, "System");

                                // Log the automated rotation initiation
                                await auditLoggingService.LogActivityAsync(new AuditLogEntry
                                {
                                    UserId = "System",
                                    Action = "AutomatedKeyRotationInitiated",
                                    ResourceId = key.Id,
                                    ResourceType = "EncryptionKey",
                                    Timestamp = DateTime.UtcNow,
                                    Sensitivity = ActivitySensitivity.High,
                                    Details = $"Automated key rotation initiated for key {key.Id}. Requires {cryptoTeam.RequiredApprovals} approvals from team members."
                                });

                                // Create alert for key custodians
                                await alertingService.CreateAlertAsync(
                                    $"Key Rotation Required - {key.Id}",
                                    $"Automated key rotation has been initiated for key {key.Id}. Requires {cryptoTeam.RequiredApprovals} team member approvals.",
                                    AlertSeverity.Medium,
                                    "KeyRotation",
                                    "System");

                                // Notify all team members
                                foreach (var member in cryptoTeam.Members)
                                {
                                    await notificationService.NotifyTeamMemberAsync(
                                        member.Id,
                                        "Key Rotation Request",
                                        $"Your approval is requested for rotating key {key.Id}.",
                                        new Dictionary<string, string>
                                        {
                                            { "KeyId", key.Id },
                                            { "RotationRequestId", rotationRequest.Id },
                                            { "RequiredApprovals", cryptoTeam.RequiredApprovals.ToString() }
                                        });
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError(ex, "Error initiating automated key rotation for key {KeyId}", key.Id);
                                
                                await alertingService.CreateAlertAsync(
                                    $"Key Rotation Failed - {key.Id}",
                                    $"Automated key rotation failed for key {key.Id}. Error: {ex.Message}",
                                    AlertSeverity.High,
                                    "KeyRotation",
                                    "System");
                            }
                        }
                    }

                    // Wait for the configured interval before checking again
                    await Task.Delay(TimeSpan.FromMinutes(_options.CheckIntervalMinutes), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in key rotation background service");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait 5 minutes before retrying on error
                }
            }
        }
    }
}
