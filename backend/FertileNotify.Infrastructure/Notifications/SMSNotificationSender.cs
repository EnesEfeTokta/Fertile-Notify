using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.Notifications
{
    public class SMSNotificationSender : INotificationSender
    {
        private readonly ILogger _logger;

        public SMSNotificationSender(ILogger<SMSNotificationSender> logger)
        {
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.SMS;

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
