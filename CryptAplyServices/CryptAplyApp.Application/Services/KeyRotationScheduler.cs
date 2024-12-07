using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using CryptAplyApp.Infrastructure.Data;
using CryptAplyApp.Application.Interfaces;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services
{
    public class KeyRotationScheduler : BackgroundService
    {
        private readonly ILogger<KeyRotationScheduler> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly KeyRotationSchedulerOptions _options;

        public KeyRotationScheduler(
            ILogger<KeyRotationScheduler> logger,
            IServiceScopeFactory scopeFactory,
            IOptions<KeyRotationSchedulerOptions> options)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _options = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Key Rotation Scheduler started at: {time}", DateTimeOffset.UtcNow);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndScheduleRotations(stoppingToken);
                    await SendRotationNotifications(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing key rotations");
                }

                await Task.Delay(_options.CheckInterval, stoppingToken);
            }
        }

        private async Task CheckAndScheduleRotations(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CryptAplyDbContext>();
            var keyManagementService = scope.ServiceProvider.GetRequiredService<IKeyManagementService>();

            var keysNeedingRotation = await dbContext.CryptoKeys
                .Where(k => k.Status == KeyStatus.Active &&
                           k.NextRotationDate.HasValue &&
                           k.NextRotationDate <= DateTime.UtcNow &&
                           !dbContext.KeyActions.Any(ka =>
                               ka.CryptoKeyId == k.Id &&
                               ka.ActionType == KeyActionType.Rotate &&
                               ka.Status == KeyActionStatus.Pending))
                .ToListAsync(stoppingToken);

            foreach (var key in keysNeedingRotation)
            {
                try
                {
                    _logger.LogInformation(
                        "Initiating scheduled rotation for key {KeyId}. Last rotated: {LastRotated}",
                        key.Id,
                        key.LastRotationDate);

                    var request = new InitiateKeyActionRequest
                    {
                        ActionType = KeyActionType.Rotate,
                        Reason = "Scheduled automatic rotation",
                        IsEmergency = false,
                        ScheduledDate = DateTime.UtcNow
                    };

                    await keyManagementService.InitiateKeyActionAsync(key.Id, request);

                    _logger.LogInformation(
                        "Successfully scheduled rotation for key {KeyId}",
                        key.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to schedule rotation for key {KeyId}",
                        key.Id);
                }
            }
        }

        private async Task SendRotationNotifications(CancellationToken stoppingToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<CryptAplyDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var warningDate = DateTime.UtcNow.Add(_options.RotationWarningThreshold);
            var keysNeedingNotification = await dbContext.CryptoKeys
                .Include(k => k.ManagingTeam)
                .ThenInclude(t => t.Members)
                .Where(k => k.Status == KeyStatus.Active &&
                           k.NextRotationDate.HasValue &&
                           k.NextRotationDate <= warningDate &&
                           !dbContext.KeyNotifications.Any(n =>
                               n.CryptoKeyId == k.Id &&
                               n.NotificationType == NotificationType.RotationWarning &&
                               n.CreatedDate >= DateTime.UtcNow.AddDays(-7)))
                .ToListAsync(stoppingToken);

            foreach (var key in keysNeedingNotification)
            {
                try
                {
                    var notification = new KeyRotationNotification
                    {
                        KeyId = key.Id,
                        KeyName = key.Name,
                        KeyDescription = key.Description,
                        ScheduledRotationDate = key.NextRotationDate,
                        LastRotationDate = key.LastRotationDate,
                        TeamMemberEmails = key.ManagingTeam.Members.Select(m => m.Email).ToList(),
                        Environment = key.Environment,
                        Applications = key.Applications?.ToDictionary(a => a.Name, a => a.Description),
                        ActionLink = GenerateActionLink(key.Id)
                    };

                    await emailService.SendKeyRotationReminderAsync(notification);

                    dbContext.KeyNotifications.Add(new KeyNotification
                    {
                        CryptoKeyId = key.Id,
                        NotificationType = NotificationType.RotationWarning,
                        CreatedDate = DateTime.UtcNow
                    });

                    await dbContext.SaveChangesAsync(stoppingToken);

                    _logger.LogInformation(
                        "Sent rotation reminder for key {KeyId}. Scheduled rotation: {RotationDate}",
                        key.Id,
                        key.NextRotationDate);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to send rotation reminder for key {KeyId}",
                        key.Id);
                }
            }
        }

        private string GenerateActionLink(int keyId)
        {
            // TODO: Generate actual action link based on your application's URL structure
            return $"/key-management/rotate/{keyId}";
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Key Rotation Scheduler is stopping");
            await base.StopAsync(stoppingToken);
        }
    }
}
