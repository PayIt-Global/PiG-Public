using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class SecurityIncident
    {
        public string IncidentId { get; set; }
        public string Title { get; set; }
        public IncidentType Type { get; set; }
        public IncidentSeverity Severity { get; set; }
        public IncidentStatus Status { get; set; }
        public DateTime DetectionTime { get; set; }
        public DateTime? ResolutionTime { get; set; }
        public string DetectedBy { get; set; }
        public string Description { get; set; }
        public List<string> AffectedSystems { get; set; }
        public List<string> AffectedKeys { get; set; }
        public List<ContainmentAction> ContainmentActions { get; set; }
        public List<ForensicEvidence> Evidence { get; set; }
        public IncidentTimeline Timeline { get; set; }
        public List<string> NotifiedParties { get; set; }
        public bool RequiresDisclosure { get; set; }
    }

    public class ContainmentAction
    {
        public string ActionId { get; set; }
        public string Description { get; set; }
        public ActionType Type { get; set; }
        public ActionStatus Status { get; set; }
        public DateTime InitiationTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public string ExecutedBy { get; set; }
        public bool IsAutomated { get; set; }
        public string Result { get; set; }
        public List<string> AffectedComponents { get; set; }
    }

    public class ForensicEvidence
    {
        public string EvidenceId { get; set; }
        public string Description { get; set; }
        public EvidenceType Type { get; set; }
        public string Location { get; set; }
        public DateTime CollectionTime { get; set; }
        public string CollectedBy { get; set; }
        public string HashValue { get; set; }
        public string ChainOfCustody { get; set; }
        public bool IsSealed { get; set; }
        public DateTime? RetentionDate { get; set; }
    }

    public class IncidentTimeline
    {
        public string TimelineId { get; set; }
        public List<TimelineEvent> Events { get; set; }
        public DateTime FirstEventTime { get; set; }
        public DateTime LastEventTime { get; set; }
        public TimeSpan TotalResolutionTime { get; set; }
        public List<string> KeyMilestones { get; set; }
    }

    public class TimelineEvent
    {
        public string EventId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public string Actor { get; set; }
        public EventCategory Category { get; set; }
        public string Evidence { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
    }

    public class AutomatedResponse
    {
        public string ResponseId { get; set; }
        public string IncidentId { get; set; }
        public ResponseType Type { get; set; }
        public List<AutomatedAction> Actions { get; set; }
        public ResponseTrigger Trigger { get; set; }
        public DateTime ExecutionTime { get; set; }
        public ResponseStatus Status { get; set; }
        public string ExecutionLog { get; set; }
    }

    public class AutomatedAction
    {
        public string ActionId { get; set; }
        public string Command { get; set; }
        public Dictionary<string, string> Parameters { get; set; }
        public int Sequence { get; set; }
        public bool RequiresApproval { get; set; }
        public string ApprovedBy { get; set; }
        public ActionStatus Status { get; set; }
        public string Result { get; set; }
    }

    public enum IncidentType
    {
        KeyCompromise,
        UnauthorizedAccess,
        SystemBreach,
        AnomalousActivity,
        ComplianceViolation,
        DataExposure
    }

    public enum IncidentSeverity
    {
        Critical,
        High,
        Medium,
        Low,
        Informational
    }

    public enum IncidentStatus
    {
        New,
        Investigating,
        Contained,
        Remediating,
        Resolved,
        Closed,
        Reopened
    }

    public enum ActionType
    {
        KeyRevocation,
        SystemIsolation,
        AccessRevocation,
        DataBackup,
        LogCollection,
        SystemRestore
    }

    public enum ActionStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        Cancelled,
        Verified
    }

    public enum EvidenceType
    {
        SystemLog,
        NetworkCapture,
        MemoryDump,
        DiskImage,
        Configuration,
        UserActivity
    }

    public enum EventCategory
    {
        Detection,
        Analysis,
        Containment,
        Eradication,
        Recovery,
        PostIncident
    }

    public enum ResponseType
    {
        Containment,
        Evidence,
        Notification,
        Remediation,
        Recovery
    }

    public enum ResponseTrigger
    {
        Automatic,
        Manual,
        Scheduled,
        Conditional
    }

    public enum ResponseStatus
    {
        Queued,
        Running,
        Completed,
        Failed,
        Cancelled,
        AwaitingApproval
    }
}
