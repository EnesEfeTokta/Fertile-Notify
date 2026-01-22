using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Infrastructure.Notifications
{
    public class ConsoleNotificationSender : INotificationSender
    {
        public NotificationChannel Channel => NotificationChannel.Console;

        public Task SendAsync(User user, string eventType, string payload)
        {
            Console.WriteLine($"[CONSOLE] To: [USER: {user}] [EVENT: {eventType}] [PAYLOAD: {payload}]");
            return Task.CompletedTask;
        }
    }
}
