using System;
using System.Collections.Generic;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class EmergencyProtocol
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public EmergencyType Type { get; set; }
        public EmergencySeverity Severity { get; set; }
        public string Procedures { get; set; }
        public string ResponsibleTeam { get; set; }
        public string EscalationPath { get; set; }
        public bool RequiresApproval { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastModifiedAt { get; set; }
        public string CreatedBy { get; set; }
        public string LastModifiedBy { get; set; }
        
        public virtual ICollection<EmergencyIncident> Incidents { get; set; }
        public virtual ICollection<ProtocolTest> Tests { get; set; }
    }

    public class EmergencyIncident
    {
        public int Id { get; set; }
        public string IncidentId { get; set; }
        public DateTime DetectionTime { get; set; }
        public DateTime? ResolutionTime { get; set; }
        public EmergencyStatus Status { get; set; }
        public string AffectedSystems { get; set; }
        public string ImpactAssessment { get; set; }
        public string ResolutionSteps { get; set; }
        public string HandledBy { get; set; }
        public Dictionary<string, string> Metadata { get; set; }
        
        public int EmergencyProtocolId { get; set; }
        public virtual EmergencyProtocol Protocol { get; set; }
        public virtual ICollection<IncidentAction> Actions { get; set; }
    }

    public class IncidentAction
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Action { get; set; }
        public string PerformedBy { get; set; }
        public ActionStatus Status { get; set; }
        public string Result { get; set; }
        public string Notes { get; set; }
        
        public int EmergencyIncidentId { get; set; }
        public virtual EmergencyIncident Incident { get; set; }
    }

    public class ProtocolTest
    {
        public int Id { get; set; }
        public DateTime TestDate { get; set; }
        public string TestScenario { get; set; }
        public TestResult Result { get; set; }
        public string ConductedBy { get; set; }
        public string Observations { get; set; }
        public string ImprovementSuggestions { get; set; }
        public Dictionary<string, string> Metrics { get; set; }
        
        public int EmergencyProtocolId { get; set; }
        public virtual EmergencyProtocol Protocol { get; set; }
    }

    public enum EmergencyType
    {
        SecurityBreach,
        HardwareFailure,
        DataCorruption,
        NetworkBreach,
        ComplianceViolation,
        SystemOverload
    }

    public enum EmergencySeverity
    {
        Low,
        Medium,
        High,
        Critical
    }

    public enum EmergencyStatus
    {
        Detected,
        InProgress,
        Contained,
        Resolved,
        PostMortem
    }

    public enum ActionStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        Cancelled
    }

    public enum TestResult
    {
        NotStarted,
        InProgress,
        Passed,
        Failed,
        PartiallyPassed
    }
}
