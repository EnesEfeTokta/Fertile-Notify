using FertileNotify.Application.Contracts;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Application.Services
{
    public class NotificationLogService : INotificationLogService
    {
        private readonly INotificationLogRepository _logRepository;
        private readonly IStatsRepository _statsRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ILogger<NotificationLogService> _logger;

        public NotificationLogService(
            INotificationLogRepository logRepository,
            IStatsRepository statsRepository,
            ISubscriptionRepository subscriptionRepository,
            ILogger<NotificationLogService> _logger)
        {
            _logRepository = logRepository;
            _statsRepository = statsRepository;
            _subscriptionRepository = subscriptionRepository;
            this._logger = _logger;
        }

        public async Task LogSuccessAsync(ProcessNotificationMessage message, string subject, string body, Subscription subscription)
        {
            var channel = NotificationChannel.From(message.Channel);
            var eventType = EventType.From(message.EventType);

            var log = new NotificationLog(
                message.SubscriberId,
                message.Recipient,
                channel,
                eventType,
                subject,
                body);

            log.SetResult(true);
            await _logRepository.AddAsync(log);

            await _statsRepository.IncrementAsync(message.SubscriberId, channel, eventType, true);
            
            subscription.IncreaseUsage();
            await _subscriptionRepository.SaveAsync(message.SubscriberId, subscription);

            _logger.LogInformation("[SUCCESS] Notification logged and usage updated for {Recipient}", message.Recipient);
        }

        public async Task LogFailureAsync(ProcessNotificationMessage message, string? subject, string? body, string errorReason)
        {
            var channel = NotificationChannel.From(message.Channel);
            var eventType = EventType.From(message.EventType);

            var log = new NotificationLog(
                message.SubscriberId,
                message.Recipient,
                channel,
                eventType,
                subject ?? "[N/A]",
                body ?? "[N/A]");

            log.SetResult(false, errorReason);
            await _logRepository.AddAsync(log);

            await _statsRepository.IncrementAsync(message.SubscriberId, channel, eventType, false);

            _logger.LogWarning("[FAILED] Notification failed for {Recipient}: {Reason}", message.Recipient, errorReason);
        }

        public async Task LogRejectedAsync(ProcessNotificationMessage message, string? subject, string? body, string errorReason)
        {
            var channel = NotificationChannel.From(message.Channel);
            var eventType = EventType.From(message.EventType);

            var log = new NotificationLog(
                message.SubscriberId,
                message.Recipient,
                channel,
                eventType,
                subject ?? "[N/A]",
                body ?? "[N/A]");

            log.SetResult(false, errorReason, isRejected: true);
            await _logRepository.AddAsync(log);
            
            _logger.LogWarning("[REJECTED] Notification rejected for {Recipient}: {Reason}", message.Recipient, errorReason);
        }
    }
}
