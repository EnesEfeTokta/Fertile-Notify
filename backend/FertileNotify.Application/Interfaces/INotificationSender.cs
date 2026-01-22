using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Interfaces
{
    public interface INotificationSender
    {
        NotificationChannel Channel { get; }
        Task SendAsync(string eventType, string payload);
    }
}