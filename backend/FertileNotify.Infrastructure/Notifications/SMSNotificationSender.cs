using FertileNotify.Application.Interfaces;
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

        public Task SendAsync(string recipient, string subject, string body)
        {
            _logger.LogInformation(
                "[SMS] Sent to: {Recipient} | Subject: {Subject} | Body: {Body}",
                recipient,
                subject,
                body
            );
            return Task.CompletedTask;
        }
    }
}
