using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
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

        public Task SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body)
        {
            _logger.LogInformation(
                "[EMAIL] Sent to: {Recipient} | Subject: {Subject} | Body: {Body}",
                recipient,
                subject,
                body
            );
            return Task.CompletedTask;
        }
    }
}