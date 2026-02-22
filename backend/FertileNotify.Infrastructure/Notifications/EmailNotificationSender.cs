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

        public async Task<bool> SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body)
        {
            try
            {
                _logger.LogInformation(
                    "[EMAIL] Sent to: {Recipient} | Subject: {Subject} | Body: {Body}",
                    recipient,
                    subject,
                    body
                );
                await Task.Delay( 1 ); // TEST
                return true;
            }
            catch { return false; }
        }
    }
}