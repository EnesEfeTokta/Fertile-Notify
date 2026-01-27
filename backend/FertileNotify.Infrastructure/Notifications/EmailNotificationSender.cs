using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.Notifications
{
    public class EmailNotificationSender : INotificationSender
    {
        private readonly ILogger<EmailNotificationSender> _logger;

        public EmailNotificationSender(ILogger<EmailNotificationSender> logger)
        {
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Email;

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