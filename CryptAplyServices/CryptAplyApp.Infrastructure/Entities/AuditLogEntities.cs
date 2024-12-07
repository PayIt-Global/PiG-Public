using System;
using System.Collections.Generic;
using System.Text.Json;
using CryptAplyApp.Application.Services;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class AuditLogEntryEntity
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
        public string MetadataJson { get; set; }
        public string UserAgent { get; set; }
        public string SessionId { get; set; }
        public bool WasSuccessful { get; set; }
        public string ErrorMessage { get; set; }

        public Dictionary<string, string> Metadata
        {
            get => string.IsNullOrEmpty(MetadataJson) ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(MetadataJson);
            set => MetadataJson = JsonSerializer.Serialize(value);
        }
    }

    public class UserLocationHistoryEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Location { get; set; }
        public DateTime FirstSeen { get; set; }
        public DateTime LastSeen { get; set; }
        public int AccessCount { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedAt { get; set; }
    }

    public class ActivityPatternEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string PatternType { get; set; }
        public string PatternValue { get; set; }
        public double Confidence { get; set; }
        public DateTime LastUpdated { get; set; }
        public string MetadataJson { get; set; }

        public Dictionary<string, string> Metadata
        {
            get => string.IsNullOrEmpty(MetadataJson) ? new Dictionary<string, string>() : JsonSerializer.Deserialize<Dictionary<string, string>>(MetadataJson);
            set => MetadataJson = JsonSerializer.Serialize(value);
        }
    }
}
