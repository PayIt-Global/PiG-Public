using System.Collections.Generic;
using System.Threading.Tasks;

namespace CryptAplyApp.Application.Services
{
    public interface IEmailTemplateService
    {
        Task SendTemplatedEmailAsync(NotificationType templateType, Dictionary<string, string> parameters, List<string> recipients);
    }

    public interface ISlackNotificationService
    {
        Task SendMessageAsync(string channel, SlackMessage message);
        Task UpdateMessageAsync(string messageTs, SlackMessage message);
    }

    public interface IPagerDutyService
    {
        Task CreateIncidentAsync(PagerDutyIncident incident);
        Task AcknowledgeIncidentAsync(string incidentId);
        Task ResolveIncidentAsync(string incidentId);
    }

    public enum NotificationType
    {
        CriticalAlert,
        HighPriorityAlert,
        MediumPriorityAlert,
        LowPriorityAlert,
        DailyDigest,
        WeeklyReport
    }
}
