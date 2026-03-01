using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.Notifications
{
    public class SlackNotificationSender : INotificationSender
    {
        private readonly INotificationLogRepository _logRepository;
        private readonly ILogger<SlackNotificationSender> _logger;

        public SlackNotificationSender(INotificationLogRepository logRepository, ILogger<SlackNotificationSender> logger)
        {
            _logRepository = logRepository;
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Slack;

        public async Task<bool> SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body, IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                _logger.LogInformation("[SLACK] Subscriber: {SubId}, Recipient: {To}", subscriberId, recipient);

                var log = new NotificationLog(
                    subscriberId,
                    recipient,
                    NotificationChannel.Console,
                    eventType,
                    subject,
                    body
                );

                await _logRepository.AddAsync(log);

                return true;
            }
            catch { return false; }
        }
    }
}
