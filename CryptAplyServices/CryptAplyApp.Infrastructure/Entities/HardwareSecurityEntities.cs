using System;
using System.Collections.Generic;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class HSMDevice
    {
        public int Id { get; set; }
        public string SerialNumber { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string FirmwareVersion { get; set; }
        public HSMStatus Status { get; set; }
        public DateTime LastHealthCheck { get; set; }
        public string Location { get; set; }
        public Dictionary<string, string> Configuration { get; set; }
        public bool IsPrimary { get; set; }
        
        public virtual ICollection<HSMPartition> Partitions { get; set; }
        public virtual ICollection<HSMHealthLog> HealthLogs { get; set; }
        public virtual ICollection<HSMOperationLog> OperationLogs { get; set; }
    }

    public class HSMPartition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public PartitionStatus Status { get; set; }
        public int KeyCapacity { get; set; }
        public int CurrentKeyCount { get; set; }
        public SecurityLevel SecurityLevel { get; set; }
        public Dictionary<string, string> Policies { get; set; }
        
        public int HSMDeviceId { get; set; }
        public virtual HSMDevice Device { get; set; }
        public virtual ICollection<HSMKey> Keys { get; set; }
    }

    public class HSMKey
    {
        public int Id { get; set; }
        public string KeyId { get; set; }
        public string Label { get; set; }
        public KeyType Type { get; set; }
        public KeyState State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string CreatedBy { get; set; }
        public Dictionary<string, string> Attributes { get; set; }
        
        public int HSMPartitionId { get; set; }
        public virtual HSMPartition Partition { get; set; }
        public virtual ICollection<KeyBackup> Backups { get; set; }
    }

    public class HSMHealthLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public HealthStatus Status { get; set; }
        public double Temperature { get; set; }
        public double PowerConsumption { get; set; }
        public int ErrorCount { get; set; }
        public string Diagnostics { get; set; }
        public Dictionary<string, double> Metrics { get; set; }
        
        public int HSMDeviceId { get; set; }
        public virtual HSMDevice Device { get; set; }
    }

    public class HSMOperationLog
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Operation { get; set; }
        public string Operator { get; set; }
        public OperationResult Result { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        
        public int HSMDeviceId { get; set; }
        public virtual HSMDevice Device { get; set; }
    }

    public class KeyBackup
    {
        public int Id { get; set; }
        public DateTime BackupTime { get; set; }
        public string BackupLocation { get; set; }
        public string BackupId { get; set; }
        public bool IsEncrypted { get; set; }
        public string EncryptionMethod { get; set; }
        public string BackedUpBy { get; set; }
        public BackupStatus Status { get; set; }
        
        public int HSMKeyId { get; set; }
        public virtual HSMKey Key { get; set; }
    }

    public enum HSMStatus
    {
        Online,
        Offline,
        Maintenance,
        Error,
        Lockdown
    }

    public enum PartitionStatus
    {
        Active,
        Inactive,
        Locked,
        Error
    }

    public enum SecurityLevel
    {
        Standard,
        High,
        VeryHigh,
        Maximum
    }

    public enum KeyType
    {
        Symmetric,
        AsymmetricPublic,
        AsymmetricPrivate,
        HMAC
    }

    public enum KeyState
    {
        PreActive,
        Active,
        Suspended,
        Deactivated,
        Compromised,
        Destroyed
    }

    public enum HealthStatus
    {
        Healthy,
        Warning,
        Critical,
        Unknown
    }

    public enum OperationResult
    {
        Success,
        Failed,
        PartialSuccess,
        Cancelled
    }

    public enum BackupStatus
    {
        InProgress,
        Completed,
        Failed,
        Verified
    }
}
