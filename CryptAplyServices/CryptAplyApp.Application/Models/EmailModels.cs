using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class KeyRotationNotification
    {
        public int KeyId { get; set; }
        public string KeyName { get; set; }
        public string KeyDescription { get; set; }
        public DateTime? ScheduledRotationDate { get; set; }
        public DateTime? LastRotationDate { get; set; }
        public List<string> TeamMemberEmails { get; set; }
        public string Environment { get; set; }
        public Dictionary<string, string> Applications { get; set; }
        public string RotationStatus { get; set; }
        public string ErrorDetails { get; set; }
        public string ActionLink { get; set; }
        public NotificationType NotificationType { get; set; }
        public NotificationPriority Priority { get; set; }
        public Dictionary<string, object> AdditionalData { get; set; }
    }

    public class EmailConfiguration
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public bool EnableSsl { get; set; } = true;
        public List<string> GlobalCcList { get; set; }
        public string EmailTemplate { get; set; }
    }

    public enum NotificationType
    {
        RotationWarning,
        RotationSuccess,
        RotationFailure,
        KeyExpiringSoon,
        KeyExpired,
        UnusedKeyAlert,
        ExcessiveUsageAlert,
        CompromiseAlert,
        AccessGranted,
        AccessRevoked,
        PolicyViolation,
        ComplianceReport,
        MaintenanceAlert,
        BackupReminder,
        EmergencyAccess,
        PciKeyRotationRequired,
        PciKeyAccessAudit,
        PciComplianceViolation,
        PciKeyDualControlEvent,
        PciKeyInventoryCheck,
        PciKeyBackupVerification,
        PciKeyCompromiseAlert,
        PciKeyUsageAlert,
        PciComplianceReport,
        PciKeyCeremonyNotification
    }

    public enum NotificationPriority
    {
        Low,
        Medium,
        High,
        Critical
    }

    public class EmailTemplate
    {
        public string Name { get; set; }
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string PlainTextBody { get; set; }
        public Dictionary<string, string> Placeholders { get; set; }
        public NotificationType NotificationType { get; set; }
        public NotificationPriority DefaultPriority { get; set; }
        public bool RequiresMFA { get; set; }
        public bool RequiresAcknowledgment { get; set; }
        public TimeSpan? ExpiresAfter { get; set; }
    }

    public class NotificationPreference
    {
        public string UserId { get; set; }
        public NotificationType NotificationType { get; set; }
        public bool EmailEnabled { get; set; }
        public bool PushEnabled { get; set; }
        public bool SlackEnabled { get; set; }
        public NotificationPriority MinimumPriority { get; set; }
        public bool DigestMode { get; set; }
        public TimeSpan? QuietHoursStart { get; set; }
        public TimeSpan? QuietHoursEnd { get; set; }
        public string TimeZone { get; set; }
    }
}
