using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class SlackConfiguration
    {
        public string WebhookUrl { get; set; }
        public string DefaultChannel { get; set; }
        public string BotName { get; set; }
        public string BotIconUrl { get; set; }
        public Dictionary<NotificationType, string> ChannelMappings { get; set; }
        public Dictionary<NotificationPriority, string> PriorityEmojis { get; set; }
    }

    public class TeamsConfiguration
    {
        public string WebhookUrl { get; set; }
        public Dictionary<NotificationType, string> WebhookMappings { get; set; }
        public bool UseAdaptiveCards { get; set; } = true;
    }

    public class NotificationChannel
    {
        public bool Email { get; set; } = true;
        public bool Slack { get; set; }
        public bool Teams { get; set; }
        public NotificationPriority MinimumPriority { get; set; }
    }

    public class SlackMessage
    {
        public string Channel { get; set; }
        public string Username { get; set; }
        public string IconUrl { get; set; }
        public List<SlackBlock> Blocks { get; set; }
        public string Text { get; set; }
    }

    public class SlackBlock
    {
        public string Type { get; set; }
        public SlackText Text { get; set; }
        public List<SlackElement> Elements { get; set; }
        public List<SlackAccessory> Accessories { get; set; }
    }

    public class SlackText
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public bool Emoji { get; set; }
    }

    public class SlackElement
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public string Style { get; set; }
        public string Value { get; set; }
        public string ActionId { get; set; }
    }

    public class SlackAccessory
    {
        public string Type { get; set; }
        public string ImageUrl { get; set; }
        public string AltText { get; set; }
    }

    public class TeamsMessage
    {
        public string Type { get; set; } = "message";
        public List<TeamsSection> Sections { get; set; }
        public List<TeamsFact> Facts { get; set; }
        public List<TeamsAction> Actions { get; set; }
        public string Summary { get; set; }
        public string ThemeColor { get; set; }
    }

    public class TeamsSection
    {
        public string ActivityTitle { get; set; }
        public string ActivitySubtitle { get; set; }
        public string ActivityImage { get; set; }
        public List<TeamsFact> Facts { get; set; }
        public bool Markdown { get; set; }
    }

    public class TeamsFact
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class TeamsAction
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Target { get; set; }
    }

    public class TeamsAdaptiveCard
    {
        public string Type { get; set; } = "AdaptiveCard";
        public string Version { get; set; } = "1.4";
        public List<TeamsAdaptiveElement> Body { get; set; }
        public List<TeamsAdaptiveAction> Actions { get; set; }
    }

    public class TeamsAdaptiveElement
    {
        public string Type { get; set; }
        public string Text { get; set; }
        public string Size { get; set; }
        public string Weight { get; set; }
        public string Color { get; set; }
        public bool Wrap { get; set; }
        public List<TeamsAdaptiveElement> Items { get; set; }
    }

    public class TeamsAdaptiveAction
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string Style { get; set; }
    }
}
