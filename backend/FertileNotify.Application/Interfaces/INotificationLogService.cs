using FertileNotify.Application.Contracts;

namespace FertileNotify.Application.Interfaces
{
    public interface INotificationLogService
    {
        Task LogSuccessAsync(
            ProcessNotificationMessage message, 
            string subject, 
            string body);

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
