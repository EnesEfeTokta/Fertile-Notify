using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Infrastructure.Notifications
{
    public class EmailNotificationSender : INotificationSender
    {
        public NotificationChannel Channel => NotificationChannel.Email;

        public Task SendAsync(string eventType, string payload)
        {
            Console.WriteLine($"[EMAIL] To: [EVENT: {eventType}] {payload}");
            return Task.CompletedTask;
        }
    }
}