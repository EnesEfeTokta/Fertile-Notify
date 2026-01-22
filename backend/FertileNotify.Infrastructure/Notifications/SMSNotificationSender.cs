using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Infrastructure.Notifications
{
    public class SMSNotificationSender : INotificationSender
    {
        public NotificationChannel Channel => NotificationChannel.SMS;

        public Task SendAsync(string eventType, string payload)
        {
            Console.WriteLine($"[SMS] To: [EVENT: {eventType}] {payload}");
            return Task.CompletedTask;
        }
    }
}
