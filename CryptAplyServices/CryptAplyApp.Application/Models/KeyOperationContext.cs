using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class KeyOperationContext
    {
        public string KeyId { get; set; }
        public string Operation { get; set; }
        public string ApplicationId { get; set; }
        public string WorkloadIdentity { get; set; }
        public string SourceIp { get; set; }
        public string DataType { get; set; }
        public bool IsHsmBacked { get; set; }
        public bool IsWrapped { get; set; }
        public bool IsPlaintext { get; set; }
        public Dictionary<string, string> Labels { get; set; }
        public string[] Classifications { get; set; }
        public Dictionary<string, string> Claims { get; set; }
        public string Environment { get; set; }
        public DateTime Timestamp { get; set; }
        public string RequestId { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class KeyCreationContext : KeyOperationContext
    {
        public string KeyOrigin { get; set; }
        public string KeySize { get; set; }
        public string KeyAlgorithm { get; set; }
        public Dictionary<string, string> KeyAttributes { get; set; }
        public string[] Approvers { get; set; }
        public bool HasMfaApproval { get; set; }
    }

    public class KeyRotationContext : KeyOperationContext
    {
        public string CurrentVersion { get; set; }
        public string NewVersion { get; set; }
        public string RotationReason { get; set; }
        public bool IsEmergencyRotation { get; set; }
        public string[] EmergencyApprovers { get; set; }
        public Dictionary<string, string> RotationMetadata { get; set; }
    }

    public class AuditEvent
    {
        public string EventType { get; set; }
        public string KeyId { get; set; }
        public string Operation { get; set; }
        public string ApplicationId { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
        public string[] Details { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }
}
