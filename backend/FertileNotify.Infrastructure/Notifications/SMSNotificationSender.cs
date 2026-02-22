using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
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

        public async Task<bool> SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body)
        {
            try
            {
                _logger.LogInformation(
                    "[SMS] Sent to: {Recipient} | Subject: {Subject} | Body: {Body}",
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
