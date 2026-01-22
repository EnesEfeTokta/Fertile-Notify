using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Infrastructure.Notifications
{
    public class EmailNotificationSender : INotificationSender
    {
        public NotificationChannel Channel => NotificationChannel.Email;

        public Task SendAsync(User user, string eventType, string payload)
        {
            Console.WriteLine($"[EMAIL] To: [USER: {user}] [EVENT: {eventType}] [PAYLOAD: {payload}]");
            return Task.CompletedTask;
        }
    }
}