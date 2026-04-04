namespace FertileNotify.Application.Interfaces.Notifications
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
