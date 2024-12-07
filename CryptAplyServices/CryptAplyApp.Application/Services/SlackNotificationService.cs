using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Services
{
    public class SlackNotificationService
    {
        private readonly ILogger<SlackNotificationService> _logger;
        private readonly HttpClient _httpClient;
        private readonly SlackConfiguration _config;

        public SlackNotificationService(
            ILogger<SlackNotificationService> logger,
            HttpClient httpClient,
            IOptions<SlackConfiguration> config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _config = config.Value;
        }

        public async Task SendNotificationAsync(KeyRotationNotification notification)
        {
            try
            {
                var message = CreateSlackMessage(notification);
                var json = JsonSerializer.Serialize(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(_config.WebhookUrl, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation(
                    "Slack notification sent successfully for key {KeyId}",
                    notification.KeyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send Slack notification for key {KeyId}",
                    notification.KeyId);
                throw;
            }
        }

        private SlackMessage CreateSlackMessage(KeyRotationNotification notification)
        {
            var channel = GetChannelForNotification(notification.NotificationType);
            var emoji = GetEmojiForPriority(notification.Priority);

            var blocks = new List<SlackBlock>
            {
                new SlackBlock
                {
                    Type = "header",
                    Text = new SlackText
                    {
                        Type = "plain_text",
                        Text = $"{emoji} {GetNotificationTitle(notification)}",
                        Emoji = true
                    }
                },
                new SlackBlock
                {
                    Type = "section",
                    Text = new SlackText
                    {
                        Type = "mrkdwn",
                        Text = GetNotificationDescription(notification)
                    }
                },
                new SlackBlock
                {
                    Type = "section",
                    Fields = GetNotificationFields(notification)
                }
            };

            if (!string.IsNullOrEmpty(notification.ActionLink))
            {
                blocks.Add(new SlackBlock
                {
                    Type = "actions",
                    Elements = new List<SlackElement>
                    {
                        new SlackElement
                        {
                            Type = "button",
                            Text = "Take Action",
                            Url = notification.ActionLink,
                            Style = notification.Priority == NotificationPriority.Critical ? "danger" : "primary"
                        }
                    }
                });
            }

            return new SlackMessage
            {
                Channel = channel,
                Username = _config.BotName,
                IconUrl = _config.BotIconUrl,
                Blocks = blocks,
                Text = GetNotificationTitle(notification) // Fallback text
            };
        }

        private string GetChannelForNotification(NotificationType type)
        {
            return _config.ChannelMappings.TryGetValue(type, out var channel)
                ? channel
                : _config.DefaultChannel;
        }

        private string GetEmojiForPriority(NotificationPriority priority)
        {
            return _config.PriorityEmojis.TryGetValue(priority, out var emoji)
                ? emoji
                : ":information_source:";
        }

        private string GetNotificationTitle(KeyRotationNotification notification)
        {
            return notification.NotificationType switch
            {
                NotificationType.RotationWarning => "Key Rotation Required",
                NotificationType.CompromiseAlert => "CRITICAL: Key Compromise Alert",
                NotificationType.KeyExpiringSoon => "Key Expiration Warning",
                NotificationType.ExcessiveUsageAlert => "Unusual Key Usage Detected",
                _ => "Key Management Notification"
            };
        }

        private string GetNotificationDescription(KeyRotationNotification notification)
        {
            var description = $"*Key:* {notification.KeyName}\n";
            description += $"*Environment:* {notification.Environment}\n";

            if (notification.ScheduledRotationDate.HasValue)
            {
                description += $"*Scheduled Date:* {notification.ScheduledRotationDate:d}\n";
            }

            if (!string.IsNullOrEmpty(notification.ErrorDetails))
            {
                description += $"*Error Details:*\n```{notification.ErrorDetails}```\n";
            }

            return description;
        }

        private List<SlackText> GetNotificationFields(KeyRotationNotification notification)
        {
            var fields = new List<SlackText>();

            if (notification.Applications?.Count > 0)
            {
                fields.Add(new SlackText
                {
                    Type = "mrkdwn",
                    Text = "*Affected Applications:*\n" + string.Join("\n", notification.Applications.Keys)
                });
            }

            if (notification.LastRotationDate.HasValue)
            {
                fields.Add(new SlackText
                {
                    Type = "mrkdwn",
                    Text = $"*Last Rotation:*\n{notification.LastRotationDate:d}"
                });
            }

            return fields;
        }
    }
}
