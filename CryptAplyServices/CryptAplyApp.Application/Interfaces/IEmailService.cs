using System.Threading.Tasks;
using CryptAplyApp.Application.Models;

namespace CryptAplyApp.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendKeyRotationNotificationAsync(KeyRotationNotification notification);
        Task SendKeyRotationReminderAsync(KeyRotationNotification notification);
        Task SendKeyRotationSuccessAsync(KeyRotationNotification notification);
        Task SendKeyRotationFailureAsync(KeyRotationNotification notification);
    }
}
