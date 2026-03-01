using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.Notifications
{
    public class MSTeamsNotificationSender : INotificationSender
    {
        private readonly INotificationLogRepository _logRepository;
        private readonly ILogger<MSTeamsNotificationSender> _logger;

        public MSTeamsNotificationSender(INotificationLogRepository logRepository, ILogger<MSTeamsNotificationSender> logger)
        {
            _logRepository = logRepository;
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.MSTeams;

        public async Task<bool> SendAsync(Guid subscriberId, string recipient, EventType eventType, string subject, string body, IReadOnlyDictionary<string, string>? providerSettings = null)
        {
            try
            {
                _logger.LogInformation("[MS TEAMS] Subscriber: {SubId}, Recipient: {To}", subscriberId, recipient);

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
