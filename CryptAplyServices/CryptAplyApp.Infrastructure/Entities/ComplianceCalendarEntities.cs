using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Infrastructure.Entities
{
    public class ComplianceCalendarEntity
    {
        [Key]
        public string CalendarId { get; set; }
        public int Year { get; set; }

        public virtual List<ComplianceEventEntity> Events { get; set; }
        public virtual List<RecurringTaskEntity> RecurringTasks { get; set; }
        public virtual List<ComplianceDeadlineEntity> Deadlines { get; set; }
        public virtual List<CalendarNotificationEntity> Notifications { get; set; }

        public string ResponsiblePartiesJson { get; set; }
    }

    public class ComplianceEventEntity
    {
        [Key]
        public string EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public EventType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EventStatus Status { get; set; }
        public string Location { get; set; }
        public bool IsHighPriority { get; set; }

        [ForeignKey("CalendarId")]
        public string CalendarId { get; set; }
        public virtual ComplianceCalendarEntity Calendar { get; set; }

        public string ParticipantsJson { get; set; }
        public string RequiredDocumentsJson { get; set; }
        public string PrerequisitesJson { get; set; }
    }

    public class RecurringTaskEntity
    {
        [Key]
        public string TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskFrequency Frequency { get; set; }
        public int FrequencyInterval { get; set; }
        public DateTime NextDueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public bool RequiresEvidence { get; set; }
        public string ComplianceRequirement { get; set; }

        [ForeignKey("CalendarId")]
        public string CalendarId { get; set; }
        public virtual ComplianceCalendarEntity Calendar { get; set; }

        public string AssignedToJson { get; set; }
        public virtual List<TaskDependencyEntity> Dependencies { get; set; }
    }

    public class ComplianceDeadlineEntity
    {
        [Key]
        public string DeadlineId { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public string Requirement { get; set; }
        public DeadlineStatus Status { get; set; }
        public string AssignedTo { get; set; }
        public bool IsFlexible { get; set; }
        public int GracePeriodDays { get; set; }

        [ForeignKey("CalendarId")]
        public string CalendarId { get; set; }
        public virtual ComplianceCalendarEntity Calendar { get; set; }

        public string PrerequisitesJson { get; set; }
        public string DependenciesJson { get; set; }
    }

    public class TaskDependencyEntity
    {
        [Key]
        public string DependencyId { get; set; }
        public string TaskId { get; set; }
        public DependencyType Type { get; set; }
        public string Description { get; set; }
        public bool IsCritical { get; set; }
        public DateTime? RequiredCompletionDate { get; set; }

        [ForeignKey("TaskId")]
        public virtual RecurringTaskEntity Task { get; set; }
    }

    public class CalendarNotificationEntity
    {
        [Key]
        public string NotificationId { get; set; }
        public string EventId { get; set; }
        public NotificationType Type { get; set; }
        public int DaysBeforeEvent { get; set; }
        public bool IsRecurring { get; set; }
        public string Message { get; set; }
        public NotificationPriority Priority { get; set; }

        [ForeignKey("CalendarId")]
        public string CalendarId { get; set; }
        public virtual ComplianceCalendarEntity Calendar { get; set; }

        public string RecipientsJson { get; set; }
        public string EscalationPathJson { get; set; }
    }

    public class ComplianceWindowEntity
    {
        [Key]
        public string WindowId { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public WindowType Type { get; set; }
        public bool RequiresApproval { get; set; }
        public string ApprovalAuthority { get; set; }

        [ForeignKey("CalendarId")]
        public string CalendarId { get; set; }
        public virtual ComplianceCalendarEntity Calendar { get; set; }

        public string AllowedActivitiesJson { get; set; }
        public string RestrictedActivitiesJson { get; set; }
    }
}
