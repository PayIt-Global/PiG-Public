using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CryptAplyApp.Application.Models;
using CryptAplyApp.Infrastructure.Data;
using CryptAplyApp.Infrastructure.Entities;

namespace CryptAplyApp.Infrastructure.Repositories
{
    public interface IComplianceCalendarRepository
    {
        Task<ComplianceCalendar> GetCalendarAsync(string calendarId);
        Task<ComplianceCalendar> GetCalendarByYearAsync(int year);
        Task<ComplianceCalendar> CreateCalendarAsync(ComplianceCalendar calendar);
        Task<ComplianceCalendar> UpdateCalendarAsync(ComplianceCalendar calendar);
        Task<bool> DeleteCalendarAsync(string calendarId);
        Task<List<ComplianceEvent>> GetUpcomingEventsAsync(DateTime startDate, DateTime endDate);
        Task<bool> AddEventAsync(string calendarId, ComplianceEvent complianceEvent);
        Task<bool> AddRecurringTaskAsync(string calendarId, RecurringTask task);
        Task<bool> AddDeadlineAsync(string calendarId, ComplianceDeadline deadline);
    }

    public class ComplianceCalendarRepository : IComplianceCalendarRepository
    {
        private readonly PciComplianceDbContext _context;

        public ComplianceCalendarRepository(PciComplianceDbContext context)
        {
            _context = context;
        }

        public async Task<ComplianceCalendar> GetCalendarAsync(string calendarId)
        {
            var calendarEntity = await _context.ComplianceCalendars
                .Include(c => c.Events)
                .Include(c => c.RecurringTasks)
                    .ThenInclude(t => t.Dependencies)
                .Include(c => c.Deadlines)
                .Include(c => c.Notifications)
                .FirstOrDefaultAsync(c => c.CalendarId == calendarId);

            return calendarEntity != null ? MapToComplianceCalendar(calendarEntity) : null;
        }

        public async Task<ComplianceCalendar> GetCalendarByYearAsync(int year)
        {
            var calendarEntity = await _context.ComplianceCalendars
                .Include(c => c.Events)
                .Include(c => c.RecurringTasks)
                    .ThenInclude(t => t.Dependencies)
                .Include(c => c.Deadlines)
                .Include(c => c.Notifications)
                .FirstOrDefaultAsync(c => c.Year == year);

            return calendarEntity != null ? MapToComplianceCalendar(calendarEntity) : null;
        }

        public async Task<ComplianceCalendar> CreateCalendarAsync(ComplianceCalendar calendar)
        {
            var calendarEntity = MapToComplianceCalendarEntity(calendar);
            _context.ComplianceCalendars.Add(calendarEntity);
            await _context.SaveChangesAsync();
            return await GetCalendarAsync(calendarEntity.CalendarId);
        }

        public async Task<ComplianceCalendar> UpdateCalendarAsync(ComplianceCalendar calendar)
        {
            var existingCalendar = await _context.ComplianceCalendars
                .Include(c => c.Events)
                .Include(c => c.RecurringTasks)
                    .ThenInclude(t => t.Dependencies)
                .Include(c => c.Deadlines)
                .Include(c => c.Notifications)
                .FirstOrDefaultAsync(c => c.CalendarId == calendar.CalendarId);

            if (existingCalendar == null)
                return null;

            UpdateComplianceCalendarEntity(existingCalendar, calendar);
            await _context.SaveChangesAsync();
            return await GetCalendarAsync(calendar.CalendarId);
        }

        public async Task<bool> DeleteCalendarAsync(string calendarId)
        {
            var calendar = await _context.ComplianceCalendars.FindAsync(calendarId);
            if (calendar == null)
                return false;

            _context.ComplianceCalendars.Remove(calendar);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<ComplianceEvent>> GetUpcomingEventsAsync(DateTime startDate, DateTime endDate)
        {
            var events = await _context.ComplianceEvents
                .Where(e => e.StartDate >= startDate && e.StartDate <= endDate)
                .OrderBy(e => e.StartDate)
                .ToListAsync();

            return events.Select(MapToComplianceEvent).ToList();
        }

        public async Task<bool> AddEventAsync(string calendarId, ComplianceEvent complianceEvent)
        {
            var calendar = await _context.ComplianceCalendars.FindAsync(calendarId);
            if (calendar == null)
                return false;

            var eventEntity = MapToComplianceEventEntity(complianceEvent);
            eventEntity.CalendarId = calendarId;
            _context.ComplianceEvents.Add(eventEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddRecurringTaskAsync(string calendarId, RecurringTask task)
        {
            var calendar = await _context.ComplianceCalendars.FindAsync(calendarId);
            if (calendar == null)
                return false;

            var taskEntity = MapToRecurringTaskEntity(task);
            taskEntity.CalendarId = calendarId;
            _context.RecurringTasks.Add(taskEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AddDeadlineAsync(string calendarId, ComplianceDeadline deadline)
        {
            var calendar = await _context.ComplianceCalendars.FindAsync(calendarId);
            if (calendar == null)
                return false;

            var deadlineEntity = MapToComplianceDeadlineEntity(deadline);
            deadlineEntity.CalendarId = calendarId;
            _context.ComplianceDeadlines.Add(deadlineEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        private ComplianceCalendar MapToComplianceCalendar(ComplianceCalendarEntity entity)
        {
            if (entity == null)
                return null;

            return new ComplianceCalendar
            {
                CalendarId = entity.CalendarId,
                Year = entity.Year,
                Events = entity.Events.Select(MapToComplianceEvent).ToList(),
                RecurringTasks = entity.RecurringTasks.Select(MapToRecurringTask).ToList(),
                Deadlines = entity.Deadlines.Select(MapToComplianceDeadline).ToList(),
                Notifications = entity.Notifications.Select(MapToCalendarNotification).ToList(),
                ResponsibleParties = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(entity.ResponsiblePartiesJson)
            };
        }

        private ComplianceCalendarEntity MapToComplianceCalendarEntity(ComplianceCalendar calendar)
        {
            if (calendar == null)
                return null;

            return new ComplianceCalendarEntity
            {
                CalendarId = string.IsNullOrEmpty(calendar.CalendarId) ? Guid.NewGuid().ToString() : calendar.CalendarId,
                Year = calendar.Year,
                Events = calendar.Events?.Select(MapToComplianceEventEntity).ToList(),
                RecurringTasks = calendar.RecurringTasks?.Select(MapToRecurringTaskEntity).ToList(),
                Deadlines = calendar.Deadlines?.Select(MapToComplianceDeadlineEntity).ToList(),
                Notifications = calendar.Notifications?.Select(MapToCalendarNotificationEntity).ToList(),
                ResponsiblePartiesJson = JsonSerializer.Serialize(calendar.ResponsibleParties)
            };
        }

        private void UpdateComplianceCalendarEntity(ComplianceCalendarEntity entity, ComplianceCalendar calendar)
        {
            entity.Year = calendar.Year;
            entity.ResponsiblePartiesJson = JsonSerializer.Serialize(calendar.ResponsibleParties);

            // Update events
            _context.ComplianceEvents.RemoveRange(entity.Events);
            entity.Events = calendar.Events?.Select(MapToComplianceEventEntity).ToList();

            // Update recurring tasks
            _context.RecurringTasks.RemoveRange(entity.RecurringTasks);
            entity.RecurringTasks = calendar.RecurringTasks?.Select(MapToRecurringTaskEntity).ToList();

            // Update deadlines
            _context.ComplianceDeadlines.RemoveRange(entity.Deadlines);
            entity.Deadlines = calendar.Deadlines?.Select(MapToComplianceDeadlineEntity).ToList();

            // Update notifications
            _context.CalendarNotifications.RemoveRange(entity.Notifications);
            entity.Notifications = calendar.Notifications?.Select(MapToCalendarNotificationEntity).ToList();
        }

        private ComplianceEvent MapToComplianceEvent(ComplianceEventEntity entity)
        {
            return new ComplianceEvent
            {
                EventId = entity.EventId,
                Title = entity.Title,
                Description = entity.Description,
                Type = entity.Type,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Status = entity.Status,
                Location = entity.Location,
                IsHighPriority = entity.IsHighPriority,
                Participants = JsonSerializer.Deserialize<List<string>>(entity.ParticipantsJson),
                RequiredDocuments = JsonSerializer.Deserialize<List<string>>(entity.RequiredDocumentsJson),
                Prerequisites = JsonSerializer.Deserialize<List<string>>(entity.PrerequisitesJson)
            };
        }

        private ComplianceEventEntity MapToComplianceEventEntity(ComplianceEvent complianceEvent)
        {
            return new ComplianceEventEntity
            {
                EventId = string.IsNullOrEmpty(complianceEvent.EventId) ? Guid.NewGuid().ToString() : complianceEvent.EventId,
                Title = complianceEvent.Title,
                Description = complianceEvent.Description,
                Type = complianceEvent.Type,
                StartDate = complianceEvent.StartDate,
                EndDate = complianceEvent.EndDate,
                Status = complianceEvent.Status,
                Location = complianceEvent.Location,
                IsHighPriority = complianceEvent.IsHighPriority,
                ParticipantsJson = JsonSerializer.Serialize(complianceEvent.Participants),
                RequiredDocumentsJson = JsonSerializer.Serialize(complianceEvent.RequiredDocuments),
                PrerequisitesJson = JsonSerializer.Serialize(complianceEvent.Prerequisites)
            };
        }

        private RecurringTask MapToRecurringTask(RecurringTaskEntity entity)
        {
            return new RecurringTask
            {
                TaskId = entity.TaskId,
                Title = entity.Title,
                Description = entity.Description,
                Frequency = entity.Frequency,
                FrequencyInterval = entity.FrequencyInterval,
                NextDueDate = entity.NextDueDate,
                Priority = entity.Priority,
                RequiresEvidence = entity.RequiresEvidence,
                ComplianceRequirement = entity.ComplianceRequirement,
                AssignedTo = JsonSerializer.Deserialize<List<string>>(entity.AssignedToJson),
                Dependencies = entity.Dependencies.Select(MapToTaskDependency).ToList()
            };
        }

        private RecurringTaskEntity MapToRecurringTaskEntity(RecurringTask task)
        {
            return new RecurringTaskEntity
            {
                TaskId = string.IsNullOrEmpty(task.TaskId) ? Guid.NewGuid().ToString() : task.TaskId,
                Title = task.Title,
                Description = task.Description,
                Frequency = task.Frequency,
                FrequencyInterval = task.FrequencyInterval,
                NextDueDate = task.NextDueDate,
                Priority = task.Priority,
                RequiresEvidence = task.RequiresEvidence,
                ComplianceRequirement = task.ComplianceRequirement,
                AssignedToJson = JsonSerializer.Serialize(task.AssignedTo),
                Dependencies = task.Dependencies?.Select(MapToTaskDependencyEntity).ToList()
            };
        }

        private ComplianceDeadline MapToComplianceDeadline(ComplianceDeadlineEntity entity)
        {
            return new ComplianceDeadline
            {
                DeadlineId = entity.DeadlineId,
                Title = entity.Title,
                DueDate = entity.DueDate,
                Requirement = entity.Requirement,
                Status = entity.Status,
                AssignedTo = entity.AssignedTo,
                IsFlexible = entity.IsFlexible,
                GracePeriodDays = entity.GracePeriodDays,
                Prerequisites = JsonSerializer.Deserialize<List<string>>(entity.PrerequisitesJson),
                Dependencies = JsonSerializer.Deserialize<List<string>>(entity.DependenciesJson)
            };
        }

        private ComplianceDeadlineEntity MapToComplianceDeadlineEntity(ComplianceDeadline deadline)
        {
            return new ComplianceDeadlineEntity
            {
                DeadlineId = string.IsNullOrEmpty(deadline.DeadlineId) ? Guid.NewGuid().ToString() : deadline.DeadlineId,
                Title = deadline.Title,
                DueDate = deadline.DueDate,
                Requirement = deadline.Requirement,
                Status = deadline.Status,
                AssignedTo = deadline.AssignedTo,
                IsFlexible = deadline.IsFlexible,
                GracePeriodDays = deadline.GracePeriodDays,
                PrerequisitesJson = JsonSerializer.Serialize(deadline.Prerequisites),
                DependenciesJson = JsonSerializer.Serialize(deadline.Dependencies)
            };
        }

        private TaskDependency MapToTaskDependency(TaskDependencyEntity entity)
        {
            return new TaskDependency
            {
                DependencyId = entity.DependencyId,
                TaskId = entity.TaskId,
                Type = entity.Type,
                Description = entity.Description,
                IsCritical = entity.IsCritical,
                RequiredCompletionDate = entity.RequiredCompletionDate
            };
        }

        private TaskDependencyEntity MapToTaskDependencyEntity(TaskDependency dependency)
        {
            return new TaskDependencyEntity
            {
                DependencyId = string.IsNullOrEmpty(dependency.DependencyId) ? Guid.NewGuid().ToString() : dependency.DependencyId,
                TaskId = dependency.TaskId,
                Type = dependency.Type,
                Description = dependency.Description,
                IsCritical = dependency.IsCritical,
                RequiredCompletionDate = dependency.RequiredCompletionDate
            };
        }

        private CalendarNotification MapToCalendarNotification(CalendarNotificationEntity entity)
        {
            return new CalendarNotification
            {
                NotificationId = entity.NotificationId,
                EventId = entity.EventId,
                Type = entity.Type,
                DaysBeforeEvent = entity.DaysBeforeEvent,
                IsRecurring = entity.IsRecurring,
                Message = entity.Message,
                Priority = entity.Priority,
                Recipients = JsonSerializer.Deserialize<List<string>>(entity.RecipientsJson),
                EscalationPath = JsonSerializer.Deserialize<List<string>>(entity.EscalationPathJson)
            };
        }

        private CalendarNotificationEntity MapToCalendarNotificationEntity(CalendarNotification notification)
        {
            return new CalendarNotificationEntity
            {
                NotificationId = string.IsNullOrEmpty(notification.NotificationId) ? Guid.NewGuid().ToString() : notification.NotificationId,
                EventId = notification.EventId,
                Type = notification.Type,
                DaysBeforeEvent = notification.DaysBeforeEvent,
                IsRecurring = notification.IsRecurring,
                Message = notification.Message,
                Priority = notification.Priority,
                RecipientsJson = JsonSerializer.Serialize(notification.Recipients),
                EscalationPathJson = JsonSerializer.Serialize(notification.EscalationPath)
            };
        }
    }
}
