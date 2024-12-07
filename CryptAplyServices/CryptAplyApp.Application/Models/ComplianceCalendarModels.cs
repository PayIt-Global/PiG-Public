using System;
using System.Collections.Generic;

namespace CryptAplyApp.Application.Models
{
    public class ComplianceCalendar
    {
        public string CalendarId { get; set; }
        public int Year { get; set; }
        public List<ComplianceEvent> Events { get; set; }
        public List<RecurringTask> RecurringTasks { get; set; }
        public List<ComplianceDeadline> Deadlines { get; set; }
        public Dictionary<string, List<string>> ResponsibleParties { get; set; }
        public List<CalendarNotification> Notifications { get; set; }
    }

    public class ComplianceEvent
    {
        public string EventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public EventType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public EventStatus Status { get; set; }
        public List<string> Participants { get; set; }
        public string Location { get; set; }
        public List<string> RequiredDocuments { get; set; }
        public List<string> Prerequisites { get; set; }
        public bool IsHighPriority { get; set; }
    }

    public class RecurringTask
    {
        public string TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskFrequency Frequency { get; set; }
        public int FrequencyInterval { get; set; }
        public DateTime NextDueDate { get; set; }
        public List<string> AssignedTo { get; set; }
        public TaskPriority Priority { get; set; }
        public List<TaskDependency> Dependencies { get; set; }
        public bool RequiresEvidence { get; set; }
        public string ComplianceRequirement { get; set; }
    }

    public class ComplianceDeadline
    {
        public string DeadlineId { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public string Requirement { get; set; }
        public DeadlineStatus Status { get; set; }
        public string AssignedTo { get; set; }
        public List<string> Prerequisites { get; set; }
        public List<string> Dependencies { get; set; }
        public bool IsFlexible { get; set; }
        public int GracePeriodDays { get; set; }
    }

    public class TaskDependency
    {
        public string DependencyId { get; set; }
        public string TaskId { get; set; }
        public DependencyType Type { get; set; }
        public string Description { get; set; }
        public bool IsCritical { get; set; }
        public DateTime? RequiredCompletionDate { get; set; }
    }

    public class CalendarNotification
    {
        public string NotificationId { get; set; }
        public string EventId { get; set; }
        public NotificationType Type { get; set; }
        public List<string> Recipients { get; set; }
        public int DaysBeforeEvent { get; set; }
        public bool IsRecurring { get; set; }
        public string Message { get; set; }
        public NotificationPriority Priority { get; set; }
        public List<string> EscalationPath { get; set; }
    }

    public class ComplianceWindow
    {
        public string WindowId { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public WindowType Type { get; set; }
        public List<string> AllowedActivities { get; set; }
        public List<string> RestrictedActivities { get; set; }
        public bool RequiresApproval { get; set; }
        public string ApprovalAuthority { get; set; }
    }

    public enum EventType
    {
        KeyRotation,
        Audit,
        Training,
        Assessment,
        Review,
        Certification,
        Maintenance
    }

    public enum EventStatus
    {
        Scheduled,
        InProgress,
        Completed,
        Cancelled,
        Postponed,
        Overdue
    }

    public enum TaskFrequency
    {
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        Annually,
        Custom
    }

    public enum TaskPriority
    {
        Critical,
        High,
        Medium,
        Low,
        Routine
    }

    public enum DeadlineStatus
    {
        Upcoming,
        Due,
        Completed,
        Missed,
        Extended,
        Waived
    }

    public enum DependencyType
    {
        Sequential,
        Parallel,
        Optional,
        Mandatory
    }

    public enum WindowType
    {
        Maintenance,
        KeyRotation,
        Audit,
        Emergency,
        Standard
    }
}
