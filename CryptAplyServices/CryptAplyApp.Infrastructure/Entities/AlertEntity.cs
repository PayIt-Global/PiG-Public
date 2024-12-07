using System;
using System.Collections.Generic;
using System.Text.Json;
using CryptAplyApp.Application.Services;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class AlertEntity
    {
        public string AlertId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Category { get; set; }
        public DateTime Timestamp { get; set; }
        public AlertStatus Status { get; set; }
        public string TagsJson { get; set; }
        public string MetadataJson { get; set; }
        public string SlackMessageTs { get; set; }
        public string AcknowledgedBy { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public string ResolvedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string Resolution { get; set; }

        public List<string> Tags
        {
            get => string.IsNullOrEmpty(TagsJson) ? new List<string>() : JsonSerializer.Deserialize<List<string>>(TagsJson);
            set => TagsJson = JsonSerializer.Serialize(value);
        }

        public Dictionary<string, string> Metadata
        {
            get => string.IsNullOrEmpty(MetadataJson) ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(MetadataJson);
            set => MetadataJson = JsonSerializer.Serialize(value);
        }
    }
}
