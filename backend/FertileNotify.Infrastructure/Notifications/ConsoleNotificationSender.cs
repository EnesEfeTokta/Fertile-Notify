using FertileNotify.Application.Interfaces;

namespace FertileNotify.Infrastructure.Notifications
{
    public class ConsoleNotificationSender : INotificationSender
    {
        public Task SendAsync(string eventType, string payload)
        {
            Console.WriteLine($"[EVENT: {eventType}] {payload}");
            return Task.CompletedTask;
        }
    }
}
