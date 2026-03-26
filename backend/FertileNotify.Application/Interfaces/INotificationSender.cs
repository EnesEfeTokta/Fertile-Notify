using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Interfaces
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