using FertileNotify.Application.Contracts;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Interfaces
{
    public interface INotificationLogService
    {
        Task LogSuccessAsync(
            ProcessNotificationMessage message, 
            NotificationContent content);

        Task LogFailureAsync(
            ProcessNotificationMessage message, 
            NotificationContent? content,
            string errorReason);

        Task LogRejectedAsync(
            ProcessNotificationMessage message,
            NotificationContent? content,
            string errorReason);
    }
}
