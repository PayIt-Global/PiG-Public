using System;
using System.Collections.Generic;

namespace CryptAplyApp.Infrastructure.Entities
{
    public enum KeyType
    {
        Encryption,
        Signing,
        MasterKey,
        DerivedKey,
        BackupKey
    }

    public enum KeyStatus
    {
        Pending,        // Created but not yet approved
        Active,         // In use
        PendingRotation,// Scheduled for rotation
        Rotating,       // Currently being rotated
        Suspended,      // Temporarily suspended
        Compromised,    // Known to be compromised
        Retired,        // No longer in use but retained
        Archived       // Archived after retention period
    }

    public class CryptoKey
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public KeyType Type { get; set; }
        public KeyStatus Status { get; set; }
        public string Version { get; set; }
        public string KeyVaultUri { get; set; }  // Reference to the key in Azure Key Vault
        public string Algorithm { get; set; }
        public int KeySizeInBits { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? LastRotationDate { get; set; }
        public DateTime? NextRotationDate { get; set; }
        public int RotationPeriodDays { get; set; }
        public int RetentionPeriodDays { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public int UsageCount { get; set; }
        public bool RequiresDoubleAuth { get; set; }  // Requires two people for usage
        public string Purpose { get; set; }
        public string Notes { get; set; }
        public string ComplianceNotes { get; set; }
        public bool IsHSMBacked { get; set; }
        public string BackupLocation { get; set; }  // For offline backup reference

        // For derived keys
        public int? ParentKeyId { get; set; }
        public virtual CryptoKey ParentKey { get; set; }
        public virtual ICollection<CryptoKey> DerivedKeys { get; set; }

        // Navigation properties
        public virtual CryptoTeam ManagingTeam { get; set; }
        public int ManagingTeamId { get; set; }
        public virtual ICollection<KeyAction> Actions { get; set; }
        public virtual ICollection<KeyUsageLog> UsageLogs { get; set; }
    }
}
