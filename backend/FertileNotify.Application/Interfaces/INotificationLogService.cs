using FertileNotify.Application.Contracts;
using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface INotificationLogService
    {
        Task LogSuccessAsync(
            ProcessNotificationMessage message, 
            string subject, 
            string body, 
            Subscription subscription);

        Task LogFailureAsync(
            ProcessNotificationMessage message, 
            string? subject, 
            string? body, 
            string errorReason);

        Task LogRejectedAsync(
            ProcessNotificationMessage message,
            string? subject,
            string? body,
            string errorReason);
    }
}
