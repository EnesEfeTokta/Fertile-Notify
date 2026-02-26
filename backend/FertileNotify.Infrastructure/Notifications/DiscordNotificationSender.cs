using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.Notifications
{
    public class DiscordNotificationSender : INotificationSender
    {
        private readonly ILogger _logger;

        public DiscordNotificationSender(ILogger<DiscordNotificationSender> logger)
        {
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Discord;

        public async Task<bool> SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body, IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                _logger.LogInformation(
                    "[Discord] Sent to: {Recipient} | Subject: {Subject} | Body: {Body}",
                    recipient,
                    subject,
                    body
                );
                await Task.Delay(1); // TEST
                return true;
            }
            catch { return false; }
        }
    }
}
