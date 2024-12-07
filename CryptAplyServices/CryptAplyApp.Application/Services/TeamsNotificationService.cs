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
    public class TeamsNotificationService
    {
        private readonly ILogger<TeamsNotificationService> _logger;
        private readonly HttpClient _httpClient;
        private readonly TeamsConfiguration _config;

        public TeamsNotificationService(
            ILogger<TeamsNotificationService> logger,
            HttpClient httpClient,
            IOptions<TeamsConfiguration> config)
        {
            _logger = logger;
            _httpClient = httpClient;
            _config = config.Value;
        }

        public async Task SendNotificationAsync(KeyRotationNotification notification)
        {
            try
            {
                var webhookUrl = GetWebhookUrl(notification.NotificationType);
                var message = _config.UseAdaptiveCards
                    ? CreateAdaptiveCard(notification)
                    : CreateTeamsMessage(notification);

                var json = JsonSerializer.Serialize(message);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(webhookUrl, content);
                response.EnsureSuccessStatusCode();

                _logger.LogInformation(
                    "Teams notification sent successfully for key {KeyId}",
                    notification.KeyId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send Teams notification for key {KeyId}",
                    notification.KeyId);
                throw;
            }
        }

        private string GetWebhookUrl(NotificationType type)
        {
            return _config.WebhookMappings.TryGetValue(type, out var webhook)
                ? webhook
                : _config.WebhookUrl;
        }

        private TeamsAdaptiveCard CreateAdaptiveCard(KeyRotationNotification notification)
        {
            var card = new TeamsAdaptiveCard
            {
                Body = new List<TeamsAdaptiveElement>
                {
                    new TeamsAdaptiveElement
                    {
                        Type = "TextBlock",
                        Size = "Large",
                        Weight = "Bolder",
                        Text = GetNotificationTitle(notification),
                        Color = GetPriorityColor(notification.Priority)
                    },
                    new TeamsAdaptiveElement
                    {
                        Type = "TextBlock",
                        Text = notification.KeyDescription,
                        Wrap = true
                    },
                    new TeamsAdaptiveElement
                    {
                        Type = "FactSet",
                        Facts = GetNotificationFacts(notification)
                    }
                },
                Actions = new List<TeamsAdaptiveAction>()
            };

            if (!string.IsNullOrEmpty(notification.ActionLink))
            {
                card.Actions.Add(new TeamsAdaptiveAction
                {
                    Type = "Action.OpenUrl",
                    Title = "Take Action",
                    Url = notification.ActionLink,
                    Style = notification.Priority == NotificationPriority.Critical ? "destructive" : "positive"
                });
            }

            return card;
        }

        private TeamsMessage CreateTeamsMessage(KeyRotationNotification notification)
        {
            var message = new TeamsMessage
            {
                ThemeColor = GetPriorityColor(notification.Priority),
                Summary = GetNotificationTitle(notification),
                Sections = new List<TeamsSection>
                {
                    new TeamsSection
                    {
                        ActivityTitle = GetNotificationTitle(notification),
                        ActivitySubtitle = $"Environment: {notification.Environment}",
                        Facts = GetMessageFacts(notification),
                        Markdown = true
                    }
                }
            };

            if (!string.IsNullOrEmpty(notification.ActionLink))
            {
                message.Actions = new List<TeamsAction>
                {
                    new TeamsAction
                    {
                        Type = "OpenUri",
                        Name = "Take Action",
                        Target = notification.ActionLink
                    }
                };
            }

            return message;
        }

        private string GetNotificationTitle(KeyRotationNotification notification)
        {
            return notification.NotificationType switch
            {
                NotificationType.RotationWarning => "🔄 Key Rotation Required",
                NotificationType.CompromiseAlert => "🚨 CRITICAL: Key Compromise Alert",
                NotificationType.KeyExpiringSoon => "⚠️ Key Expiration Warning",
                NotificationType.ExcessiveUsageAlert => "📊 Unusual Key Usage Detected",
                _ => "Key Management Notification"
            };
        }

        private string GetPriorityColor(NotificationPriority priority)
        {
            return priority switch
            {
                NotificationPriority.Critical => "attention",
                NotificationPriority.High => "warning",
                NotificationPriority.Medium => "accent",
                _ => "default"
            };
        }

        private List<TeamsFact> GetMessageFacts(KeyRotationNotification notification)
        {
            var facts = new List<TeamsFact>
            {
                new TeamsFact { Name = "Key Name", Value = notification.KeyName },
                new TeamsFact { Name = "Environment", Value = notification.Environment }
            };

            if (notification.ScheduledRotationDate.HasValue)
            {
                facts.Add(new TeamsFact
                {
                    Name = "Scheduled Date",
                    Value = notification.ScheduledRotationDate.Value.ToString("d")
                });
            }

            if (notification.LastRotationDate.HasValue)
            {
                facts.Add(new TeamsFact
                {
                    Name = "Last Rotation",
                    Value = notification.LastRotationDate.Value.ToString("d")
                });
            }

            if (notification.Applications?.Count > 0)
            {
                facts.Add(new TeamsFact
                {
                    Name = "Affected Applications",
                    Value = string.Join(", ", notification.Applications.Keys)
                });
            }

            return facts;
        }

        private List<TeamsAdaptiveElement> GetNotificationFacts(KeyRotationNotification notification)
        {
            var facts = new List<TeamsAdaptiveElement>();

            foreach (var fact in GetMessageFacts(notification))
            {
                facts.Add(new TeamsAdaptiveElement
                {
                    Type = "TextBlock",
                    Text = $"**{fact.Name}:** {fact.Value}",
                    Wrap = true
                });
            }

            return facts;
        }
    }
}
