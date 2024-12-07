using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CryptAplyApp.Application.Services
{
    public interface IAlertingService
    {
        Task SendAlertAsync(Alert alert);
        Task<List<Alert>> GetActiveAlertsAsync();
        Task AcknowledgeAlertAsync(string alertId, string acknowledgedBy);
        Task ResolveAlertAsync(string alertId, string resolvedBy, string resolution);
    }

    public class AlertingService : IAlertingService
    {
        private readonly ILogger<AlertingService> _logger;
        private readonly AlertingOptions _options;
        private readonly IEmailTemplateService _emailService;
        private readonly ISlackNotificationService _slackService;
        private readonly IPagerDutyService _pagerDutyService;

        public AlertingService(
            ILogger<AlertingService> logger,
            IOptions<AlertingOptions> options,
            IEmailTemplateService emailService,
            ISlackNotificationService slackService,
            IPagerDutyService pagerDutyService)
        {
            _logger = logger;
            _options = options.Value;
            _emailService = emailService;
            _slackService = slackService;
            _pagerDutyService = pagerDutyService;
        }

        public async Task SendAlertAsync(Alert alert)
        {
            try
            {
                _logger.LogInformation($"Sending alert: {alert.Title}");

                // Generate alert ID if not provided
                if (string.IsNullOrEmpty(alert.AlertId))
                {
                    alert.AlertId = Guid.NewGuid().ToString();
                }

                // Set timestamp if not provided
                if (alert.Timestamp == default)
                {
                    alert.Timestamp = DateTime.UtcNow;
                }

                // Store alert
                await StoreAlertAsync(alert);

                // Send notifications based on severity
                switch (alert.Severity)
                {
                    case AlertSeverity.Critical:
                        await SendCriticalAlertAsync(alert);
                        break;

                    case AlertSeverity.High:
                        await SendHighPriorityAlertAsync(alert);
                        break;

                    case AlertSeverity.Medium:
                        await SendMediumPriorityAlertAsync(alert);
                        break;

                    case AlertSeverity.Low:
                        await SendLowPriorityAlertAsync(alert);
                        break;
                }

                _logger.LogInformation($"Alert sent successfully: {alert.AlertId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending alert: {alert.Title}");
                throw;
            }
        }

        public async Task<List<Alert>> GetActiveAlertsAsync()
        {
            try
            {
                return await GetActiveAlertsFromStorageAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active alerts");
                throw;
            }
        }

        public async Task AcknowledgeAlertAsync(string alertId, string acknowledgedBy)
        {
            try
            {
                var alert = await GetAlertAsync(alertId);
                if (alert == null)
                    throw new KeyNotFoundException($"Alert {alertId} not found");

                alert.Status = AlertStatus.Acknowledged;
                alert.AcknowledgedBy = acknowledgedBy;
                alert.AcknowledgedAt = DateTime.UtcNow;

                await UpdateAlertAsync(alert);

                // Send acknowledgment notification
                await SendAcknowledgmentNotificationAsync(alert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error acknowledging alert {alertId}");
                throw;
            }
        }

        public async Task ResolveAlertAsync(string alertId, string resolvedBy, string resolution)
        {
            try
            {
                var alert = await GetAlertAsync(alertId);
                if (alert == null)
                    throw new KeyNotFoundException($"Alert {alertId} not found");

                alert.Status = AlertStatus.Resolved;
                alert.ResolvedBy = resolvedBy;
                alert.ResolvedAt = DateTime.UtcNow;
                alert.Resolution = resolution;

                await UpdateAlertAsync(alert);

                // Send resolution notification
                await SendResolutionNotificationAsync(alert);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error resolving alert {alertId}");
                throw;
            }
        }

        private async Task SendCriticalAlertAsync(Alert alert)
        {
            // Send PagerDuty incident
            await _pagerDutyService.CreateIncidentAsync(new PagerDutyIncident
            {
                Title = alert.Title,
                Description = alert.Description,
                Severity = "critical",
                Source = "CryptAplyApp",
                CustomDetails = alert.Metadata
            });

            // Send Slack notification to critical alerts channel
            await _slackService.SendMessageAsync(_options.CriticalAlertsChannel, new SlackMessage
            {
                Text = $":rotating_light: *CRITICAL ALERT*: {alert.Title}",
                Blocks = BuildSlackBlocks(alert)
            });

            // Send email to critical response team
            await _emailService.SendTemplatedEmailAsync(
                NotificationType.CriticalAlert,
                new Dictionary<string, string>
                {
                    { "AlertId", alert.AlertId },
                    { "Title", alert.Title },
                    { "Description", alert.Description },
                    { "Category", alert.Category },
                    { "Timestamp", alert.Timestamp.ToString("yyyy-MM-dd HH:mm:ss UTC") }
                },
                _options.CriticalResponseTeam
            );
        }

        private async Task SendHighPriorityAlertAsync(Alert alert)
        {
            // Send PagerDuty incident for business hours
            if (IsWithinBusinessHours())
            {
                await _pagerDutyService.CreateIncidentAsync(new PagerDutyIncident
                {
                    Title = alert.Title,
                    Description = alert.Description,
                    Severity = "high",
                    Source = "CryptAplyApp",
                    CustomDetails = alert.Metadata
                });
            }

            // Send Slack notification
            await _slackService.SendMessageAsync(_options.HighPriorityAlertsChannel, new SlackMessage
            {
                Text = $":warning: *High Priority Alert*: {alert.Title}",
                Blocks = BuildSlackBlocks(alert)
            });

            // Send email
            await _emailService.SendTemplatedEmailAsync(
                NotificationType.HighPriorityAlert,
                new Dictionary<string, string>
                {
                    { "AlertId", alert.AlertId },
                    { "Title", alert.Title },
                    { "Description", alert.Description },
                    { "Category", alert.Category },
                    { "Timestamp", alert.Timestamp.ToString("yyyy-MM-dd HH:mm:ss UTC") }
                },
                _options.SecurityTeam
            );
        }

        private async Task SendMediumPriorityAlertAsync(Alert alert)
        {
            // Send Slack notification
            await _slackService.SendMessageAsync(_options.MediumPriorityAlertsChannel, new SlackMessage
            {
                Text = $":bell: Medium Priority Alert: {alert.Title}",
                Blocks = BuildSlackBlocks(alert)
            });

            // Send email
            await _emailService.SendTemplatedEmailAsync(
                NotificationType.MediumPriorityAlert,
                new Dictionary<string, string>
                {
                    { "AlertId", alert.AlertId },
                    { "Title", alert.Title },
                    { "Description", alert.Description },
                    { "Category", alert.Category },
                    { "Timestamp", alert.Timestamp.ToString("yyyy-MM-dd HH:mm:ss UTC") }
                },
                _options.ComplianceTeam
            );
        }

        private async Task SendLowPriorityAlertAsync(Alert alert)
        {
            // Send Slack notification
            await _slackService.SendMessageAsync(_options.LowPriorityAlertsChannel, new SlackMessage
            {
                Text = $":information_source: Low Priority Alert: {alert.Title}",
                Blocks = BuildSlackBlocks(alert)
            });

            // Send email for daily digest
            await StoreAlertForDigestAsync(alert);
        }

        private List<SlackBlock> BuildSlackBlocks(Alert alert)
        {
            var blocks = new List<SlackBlock>
            {
                new SlackBlock
                {
                    Type = "section",
                    Text = new SlackText
                    {
                        Type = "mrkdwn",
                        Text = $"*{alert.Title}*\n{alert.Description}"
                    }
                },
                new SlackBlock
                {
                    Type = "section",
                    Fields = new List<SlackField>
                    {
                        new SlackField { Text = $"*Category:* {alert.Category}" },
                        new SlackField { Text = $"*Severity:* {alert.Severity}" },
                        new SlackField { Text = $"*Alert ID:* {alert.AlertId}" },
                        new SlackField { Text = $"*Time:* {alert.Timestamp:yyyy-MM-dd HH:mm:ss UTC}" }
                    }
                }
            };

            if (alert.Metadata?.Count > 0)
            {
                blocks.Add(new SlackBlock
                {
                    Type = "section",
                    Text = new SlackText
                    {
                        Type = "mrkdwn",
                        Text = "*Additional Details:*\n" + string.Join("\n", alert.Metadata.Select(kv => $"• {kv.Key}: {kv.Value}"))
                    }
                });
            }

            blocks.Add(new SlackBlock
            {
                Type = "actions",
                Elements = new List<SlackElement>
                {
                    new SlackElement
                    {
                        Type = "button",
                        Text = new SlackText { Type = "plain_text", Text = "Acknowledge" },
                        Value = alert.AlertId,
                        ActionId = "acknowledge_alert"
                    },
                    new SlackElement
                    {
                        Type = "button",
                        Text = new SlackText { Type = "plain_text", Text = "View Details" },
                        Url = $"{_options.AlertManagementUrl}/alerts/{alert.AlertId}",
                        ActionId = "view_alert"
                    }
                }
            });

            return blocks;
        }

        private async Task SendAcknowledgmentNotificationAsync(Alert alert)
        {
            // Update PagerDuty if it's a critical or high priority alert
            if (alert.Severity == AlertSeverity.Critical || alert.Severity == AlertSeverity.High)
            {
                await _pagerDutyService.AcknowledgeIncidentAsync(alert.AlertId);
            }

            // Send Slack update
            await _slackService.UpdateMessageAsync(alert.SlackMessageTs, new SlackMessage
            {
                Text = $"Alert Acknowledged: {alert.Title}",
                Blocks = BuildAcknowledgedSlackBlocks(alert)
            });
        }

        private async Task SendResolutionNotificationAsync(Alert alert)
        {
            // Update PagerDuty if it's a critical or high priority alert
            if (alert.Severity == AlertSeverity.Critical || alert.Severity == AlertSeverity.High)
            {
                await _pagerDutyService.ResolveIncidentAsync(alert.AlertId);
            }

            // Send Slack update
            await _slackService.UpdateMessageAsync(alert.SlackMessageTs, new SlackMessage
            {
                Text = $"Alert Resolved: {alert.Title}",
                Blocks = BuildResolvedSlackBlocks(alert)
            });
        }

        private List<SlackBlock> BuildAcknowledgedSlackBlocks(Alert alert)
        {
            var blocks = BuildSlackBlocks(alert);
            blocks.Add(new SlackBlock
            {
                Type = "context",
                Elements = new List<SlackElement>
                {
                    new SlackElement
                    {
                        Type = "mrkdwn",
                        Text = $"Acknowledged by {alert.AcknowledgedBy} at {alert.AcknowledgedAt:yyyy-MM-dd HH:mm:ss UTC}"
                    }
                }
            });
            return blocks;
        }

        private List<SlackBlock> BuildResolvedSlackBlocks(Alert alert)
        {
            var blocks = BuildSlackBlocks(alert);
            blocks.Add(new SlackBlock
            {
                Type = "context",
                Elements = new List<SlackElement>
                {
                    new SlackElement
                    {
                        Type = "mrkdwn",
                        Text = $"Resolved by {alert.ResolvedBy} at {alert.ResolvedAt:yyyy-MM-dd HH:mm:ss UTC}\nResolution: {alert.Resolution}"
                    }
                }
            });
            return blocks;
        }

        private bool IsWithinBusinessHours()
        {
            var now = DateTime.UtcNow;
            var hour = now.Hour;
            return now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Friday
                && hour >= _options.BusinessHoursStart && hour < _options.BusinessHoursEnd;
        }

        private Task StoreAlertAsync(Alert alert)
        {
            // Implementation depends on your storage mechanism
            throw new NotImplementedException();
        }

        private Task<List<Alert>> GetActiveAlertsFromStorageAsync()
        {
            // Implementation depends on your storage mechanism
            throw new NotImplementedException();
        }

        private Task<Alert> GetAlertAsync(string alertId)
        {
            // Implementation depends on your storage mechanism
            throw new NotImplementedException();
        }

        private Task UpdateAlertAsync(Alert alert)
        {
            // Implementation depends on your storage mechanism
            throw new NotImplementedException();
        }

        private Task StoreAlertForDigestAsync(Alert alert)
        {
            // Implementation depends on your storage mechanism
            throw new NotImplementedException();
        }
    }

    public class AlertingOptions
    {
        public string CriticalAlertsChannel { get; set; }
        public string HighPriorityAlertsChannel { get; set; }
        public string MediumPriorityAlertsChannel { get; set; }
        public string LowPriorityAlertsChannel { get; set; }
        public List<string> CriticalResponseTeam { get; set; }
        public List<string> SecurityTeam { get; set; }
        public List<string> ComplianceTeam { get; set; }
        public string AlertManagementUrl { get; set; }
        public int BusinessHoursStart { get; set; } = 9;  // 9 AM UTC
        public int BusinessHoursEnd { get; set; } = 17;   // 5 PM UTC
    }

    public class Alert
    {
        public string AlertId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Category { get; set; }
        public DateTime Timestamp { get; set; }
        public AlertStatus Status { get; set; }
        public List<string> Tags { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        public string SlackMessageTs { get; set; }
        public string AcknowledgedBy { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public string ResolvedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string Resolution { get; set; }
    }

    public enum AlertType
    {
        HighRisk,
        RiskTrend,
        CriticalIncident,
        PendingActions,
        DeadlineApproaching,
        DeadlineMissed,
        KeyRotation,
        KeyAge,
        SuspiciousActivity,
        FailedOperation
    }

    public enum AlertSeverity
    {
        Critical,
        High,
        Medium,
        Low
    }

    public enum AlertStatus
    {
        New,
        Acknowledged,
        Resolved
    }

    public class SlackMessage
    {
        public string Text { get; set; }
        public List<SlackBlock> Blocks { get; set; }
    }

    public class SlackBlock
    {
        public string Type { get; set; }
        public SlackText Text { get; set; }
        public List<SlackField> Fields { get; set; }
        public List<SlackElement> Elements { get; set; }
    }

    public class SlackText
    {
        public string Type { get; set; }
        public string Text { get; set; }
    }

    public class SlackField
    {
        public string Text { get; set; }
    }

    public class SlackElement
    {
        public string Type { get; set; }
        public SlackText Text { get; set; }
        public string Value { get; set; }
        public string ActionId { get; set; }
        public string Url { get; set; }
    }

    public class PagerDutyIncident
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string Source { get; set; }
        public Dictionary<string, string> CustomDetails { get; set; }
    }
}
