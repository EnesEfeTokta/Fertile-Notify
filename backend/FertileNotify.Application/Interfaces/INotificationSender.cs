using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Interfaces
{
    public interface INotificationSender
    {
        NotificationChannel Channel { get; }
        Task SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body);
    }
}