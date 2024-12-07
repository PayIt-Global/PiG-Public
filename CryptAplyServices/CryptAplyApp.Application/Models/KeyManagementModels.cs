using System;
using System.Collections.Generic;
using CryptAplyApp.Infrastructure.Entities;

namespace CryptAplyApp.Application.Models
{
    public class CryptoKeyDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public KeyType Type { get; set; }
        public KeyStatus Status { get; set; }
        public string Version { get; set; }
        public string Algorithm { get; set; }
        public int KeySizeInBits { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? LastRotationDate { get; set; }
        public DateTime? NextRotationDate { get; set; }
        public bool RequiresDoubleAuth { get; set; }
        public bool IsHSMBacked { get; set; }
        public int UsageCount { get; set; }
        public string ManagingTeamName { get; set; }
    }

    public class CreateKeyRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public KeyType Type { get; set; }
        public string Algorithm { get; set; }
        public int KeySizeInBits { get; set; }
        public int ManagingTeamId { get; set; }
        public bool RequiresDoubleAuth { get; set; }
        public bool IsHSMBacked { get; set; }
        public int? ParentKeyId { get; set; }
        public int RotationPeriodDays { get; set; }
        public int RetentionPeriodDays { get; set; }
    }

    public class RotateKeyRequest
    {
        public string Reason { get; set; }
        public bool IsEmergency { get; set; }
        public DateTime? ScheduledDate { get; set; }
    }

    public class CompromiseKeyRequest
    {
        public string IncidentDescription { get; set; }
        public DateTime CompromiseDate { get; set; }
        public string ContainmentActions { get; set; }
        public bool RequiresImmediateAction { get; set; }
    }

    public class KeyActionDto
    {
        public int Id { get; set; }
        public KeyActionType Type { get; set; }
        public KeyActionStatus Status { get; set; }
        public DateTime RequestDate { get; set; }
        public string Reason { get; set; }
        public bool RequiresQuorum { get; set; }
        public int RequiredVotes { get; set; }
        public int CurrentVotes { get; set; }
        public bool IsEmergency { get; set; }
        public string InitiatorName { get; set; }
        public List<KeyActionVoteDto> Votes { get; set; }
    }

    public class CreateKeyActionRequest
    {
        public int KeyId { get; set; }
        public KeyActionType Type { get; set; }
        public string Reason { get; set; }
        public bool IsEmergency { get; set; }
        public string Details { get; set; }
    }

    public class KeyActionVoteRequest
    {
        public bool Approved { get; set; }
        public string Comment { get; set; }
        public bool UseMFA { get; set; }
    }

    public class KeyActionStatusDto
    {
        public KeyActionStatus Status { get; set; }
        public int ApprovalCount { get; set; }
        public int RejectionCount { get; set; }
        public bool HasReachedQuorum { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Result { get; set; }
    }

    public class KeyActionVoteDto
    {
        public string VoterName { get; set; }
        public bool Approved { get; set; }
        public DateTime VoteDate { get; set; }
        public bool UsedMFA { get; set; }
    }

    public class KeyMetricsDto
    {
        public long TotalUsageCount { get; set; }
        public DateTime? LastUsed { get; set; }
        public int PendingActionsCount { get; set; }
        public DateTime? NextScheduledRotation { get; set; }
        public int DaysUntilExpiry { get; set; }
        public List<KeyUsageLogDto> RecentUsage { get; set; }
    }

    public class KeyUsageLogDto
    {
        public DateTime Timestamp { get; set; }
        public string Operation { get; set; }
        public string Application { get; set; }
        public bool WasSuccessful { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class ValidateKeyUsageRequest
    {
        public string Operation { get; set; }
        public string Application { get; set; }
        public bool RequiresMFA { get; set; }
    }

    public class EmergencyProcedureRequest
    {
        public string Reason { get; set; }
        public string ApproverName { get; set; }
        public string EmergencyContact { get; set; }
        public string ContainmentSteps { get; set; }
    }
}
