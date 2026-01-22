using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Infrastructure.Notifications
{
    public class SMSNotificationSender : INotificationSender
    {
        public NotificationChannel Channel => NotificationChannel.SMS;

        public Task SendAsync(User user, string eventType, string payload)
        {
            Console.WriteLine($"[SMS] To: [USER: {user}] [EVENT: {eventType}] [PAYLOAD: {payload}]");
            return Task.CompletedTask;
        }
    }
}
