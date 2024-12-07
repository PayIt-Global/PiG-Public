using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class SecurityIncidentEntity
    {
        [Key]
        public string IncidentId { get; set; }
        public string Title { get; set; }
        public IncidentType Type { get; set; }
        public IncidentSeverity Severity { get; set; }
        public IncidentStatus Status { get; set; }
        public DateTime DetectionTime { get; set; }
        public DateTime? ResolutionTime { get; set; }
        public string DetectedBy { get; set; }
        public string Description { get; set; }
        public bool RequiresDisclosure { get; set; }

        public string AffectedSystemsJson { get; set; }
        public string AffectedKeysJson { get; set; }
        public string NotifiedPartiesJson { get; set; }

        public virtual List<ContainmentActionEntity> ContainmentActions { get; set; }
        public virtual List<ForensicEvidenceEntity> Evidence { get; set; }
        public virtual IncidentTimelineEntity Timeline { get; set; }
    }

    public class ContainmentActionEntity
    {
        [Key]
        public string ActionId { get; set; }
        public string Description { get; set; }
        public ActionType Type { get; set; }
        public ActionStatus Status { get; set; }
        public DateTime InitiationTime { get; set; }
        public DateTime? CompletionTime { get; set; }
        public string ExecutedBy { get; set; }
        public bool IsAutomated { get; set; }
        public string Result { get; set; }

        [ForeignKey("IncidentId")]
        public string IncidentId { get; set; }
        public virtual SecurityIncidentEntity Incident { get; set; }

        public string AffectedComponentsJson { get; set; }
    }

    public class ForensicEvidenceEntity
    {
        [Key]
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

        [ForeignKey("IncidentId")]
        public string IncidentId { get; set; }
        public virtual SecurityIncidentEntity Incident { get; set; }
    }

    public class IncidentTimelineEntity
    {
        [Key]
        public string TimelineId { get; set; }
        public DateTime FirstEventTime { get; set; }
        public DateTime LastEventTime { get; set; }
        public TimeSpan TotalResolutionTime { get; set; }

        [ForeignKey("IncidentId")]
        public string IncidentId { get; set; }
        public virtual SecurityIncidentEntity Incident { get; set; }

        public virtual List<TimelineEventEntity> Events { get; set; }
        public string KeyMilestonesJson { get; set; }
    }

    public class TimelineEventEntity
    {
        [Key]
        public string EventId { get; set; }
        public DateTime Timestamp { get; set; }
        public string Description { get; set; }
        public string Actor { get; set; }
        public EventCategory Category { get; set; }
        public string Evidence { get; set; }

        [ForeignKey("TimelineId")]
        public string TimelineId { get; set; }
        public virtual IncidentTimelineEntity Timeline { get; set; }

        public string MetadataJson { get; set; }
    }

    public class AutomatedResponseEntity
    {
        [Key]
        public string ResponseId { get; set; }
        public ResponseType Type { get; set; }
        public ResponseTrigger Trigger { get; set; }
        public DateTime ExecutionTime { get; set; }
        public ResponseStatus Status { get; set; }
        public string ExecutionLog { get; set; }

        [ForeignKey("IncidentId")]
        public string IncidentId { get; set; }
        public virtual SecurityIncidentEntity Incident { get; set; }

        public virtual List<AutomatedActionEntity> Actions { get; set; }
    }

    public class AutomatedActionEntity
    {
        [Key]
        public string ActionId { get; set; }
        public string Command { get; set; }
        public int Sequence { get; set; }
        public bool RequiresApproval { get; set; }
        public string ApprovedBy { get; set; }
        public ActionStatus Status { get; set; }
        public string Result { get; set; }

        [ForeignKey("ResponseId")]
        public string ResponseId { get; set; }
        public virtual AutomatedResponseEntity Response { get; set; }

        public string ParametersJson { get; set; }
    }
}
