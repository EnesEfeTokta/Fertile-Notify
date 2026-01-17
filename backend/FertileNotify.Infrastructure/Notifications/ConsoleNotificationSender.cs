using FertileNotify.Domain.Entities;
using FertileNotify.Application.Interfaces;

namespace FertileNotify.Infrastructure.Notifications
{
    public class ConsoleNotificationSender : INotificationRepository
    {
        public Task SendAsync(Notification notification)
        {
            Console.WriteLine($"=== Notification Sent ===");
            Console.WriteLine($"ID: {notification.Id}");
            Console.WriteLine($"Title: {notification.Title}");
            Console.WriteLine($"Title: {notification.Message}");
            Console.WriteLine($"Create At: {notification.CreateAt}");
            return Task.CompletedTask;
        }
    }
}
