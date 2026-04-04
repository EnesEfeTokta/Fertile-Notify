namespace FertileNotify.Application.Interfaces.Notifications
{
    public interface INotificationSender
    {
        NotificationChannel Channel { get; }
        Task<bool> SendAsync(
            Guid subscriberId, 
            string recipient, 
            EventType eventType, 
            NotificationContent content, 
            IReadOnlyDictionary<string, string>? providerSettings = null);
    }
}