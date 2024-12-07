using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class KeyCeremony
    {
        public string CeremonyId { get; set; }
        public CeremonyType Type { get; set; }
        public DateTime ScheduledDateTime { get; set; }
        public string Location { get; set; }
        public TimeSpan EstimatedDuration { get; set; }
        public string KeyPurpose { get; set; }
        public CeremonyStatus Status { get; set; }
        public List<CeremonyParticipant> RequiredParticipants { get; set; }
        public List<SecurityRequirement> SecurityRequirements { get; set; }
        public List<CeremonyDocument> RequiredDocuments { get; set; }
        public List<CeremonyStep> Steps { get; set; }
        public string HsmIdentifier { get; set; }
        public List<string> BackupMaterials { get; set; }
        public AuditLog AuditLog { get; set; }
    }

    public class CeremonyParticipant
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public ParticipantRole Role { get; set; }
        public bool HasConfirmed { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public List<string> RequiredCredentials { get; set; }
        public bool IsPrimary { get; set; }
        public string BackupParticipantId { get; set; }
        public List<string> AssignedComponents { get; set; }
    }

    public class SecurityRequirement
    {
        public string RequirementId { get; set; }
        public string Description { get; set; }
        public bool IsMandatory { get; set; }
        public string VerificationMethod { get; set; }
        public bool IsVerified { get; set; }
        public DateTime? VerificationDate { get; set; }
        public string VerifiedBy { get; set; }
    }

    public class CeremonyDocument
    {
        public string DocumentId { get; set; }
        public string Name { get; set; }
        public DocumentType Type { get; set; }
        public string Template { get; set; }
        public bool RequiresSignature { get; set; }
        public List<string> RequiredSigners { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string StorageLocation { get; set; }
        public string RetentionPeriod { get; set; }
    }

    public class CeremonyStep
    {
        public string StepId { get; set; }
        public int Sequence { get; set; }
        public string Description { get; set; }
        public List<string> ResponsibleParties { get; set; }
        public bool RequiresDualControl { get; set; }
        public bool RequiresWitness { get; set; }
        public StepStatus Status { get; set; }
        public DateTime? CompletionTime { get; set; }
        public string CompletedBy { get; set; }
        public string VerifiedBy { get; set; }
        public string Notes { get; set; }
    }

    public class AuditLog
    {
        public string CeremonyId { get; set; }
        public List<AuditEntry> Entries { get; set; }
    }

    public class AuditEntry
    {
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string PerformedBy { get; set; }
        public string Details { get; set; }
        public string Evidence { get; set; }
    }

    public enum CeremonyType
    {
        KeyGeneration,
        KeyRotation,
        BackupCreation,
        EmergencyRecovery,
        HsmInitialization,
        ComponentSplit
    }

    public enum CeremonyStatus
    {
        Planned,
        ParticipantsConfirmed,
        InProgress,
        Completed,
        Failed,
        Cancelled,
        PostponedPending
    }

    public enum ParticipantRole
    {
        KeyCustodian,
        SecurityOfficer,
        Witness,
        Auditor,
        SystemAdministrator,
        EmergencyContact
    }

    public enum DocumentType
    {
        CeremonyScript,
        SplitKnowledgeForm,
        ChainOfCustody,
        KeyComponentForm,
        WitnessAttestation,
        AuditLog,
        ComplianceEvidence
    }

    public enum StepStatus
    {
        Pending,
        InProgress,
        Completed,
        Verified,
        Failed,
        Skipped
    }
}
