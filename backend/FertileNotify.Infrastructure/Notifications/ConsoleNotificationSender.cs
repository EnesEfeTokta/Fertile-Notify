using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.Notifications
{
    public class ConsoleNotificationSender : INotificationSender
    {
        private readonly ILogger<ConsoleNotificationSender> _logger;

        public ConsoleNotificationSender(ILogger<ConsoleNotificationSender> logger)
        {
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Console;

        public Task SendAsync(User user, string eventType, string payload)
        {
            _logger.LogInformation(
                "[CONSOLE] To: [USER: {User}] [EVENT: {EventType}] [PAYLOAD: {Payload}]",
                user,
                eventType,
                payload
            );
            return Task.CompletedTask;
        }
    }
}
