using StackExchange.Redis;
using System.Text.Json;

namespace FertileNotify.Infrastructure.BackgroundJobs.Notifications
{
    public class RedisNotificationLogService : INotificationLogService
    {
        private readonly IStatsRepository _statsRepository;
        private readonly ILogger<RedisNotificationLogService> _logger;
        private readonly IDatabase _redisDb;

        public RedisNotificationLogService(
            IStatsRepository statsRepository,
            ILogger<RedisNotificationLogService> _logger,
            IConnectionMultiplexer redis)
        {
            _statsRepository = statsRepository;
            this._logger = _logger;
            _redisDb = redis.GetDatabase();
        }

        public async Task LogSuccessAsync(ProcessNotificationMessage message, NotificationContent content)
        {
            var queueItem = NotificationLogQueueItem.Success(message, content);
            var queueLength = await _redisDb.ListLeftPushAsync("log_queue", JsonSerializer.Serialize(queueItem));

            await _statsRepository.IncrementAsync(
                message.SubscriberId,
                NotificationChannel.From(message.Channel),
                Domain.Events.EventType.From(message.EventType),
                true);

            _logger.LogInformation("[QUEUE] Success log queued. Recipient={Recipient}, QueueLength={QueueLength}", message.Recipient, queueLength);
            _logger.LogInformation("[SUCCESS] Notification logged and usage updated for {Recipient}", message.Recipient);
        }

        public async Task LogFailureAsync(ProcessNotificationMessage message, NotificationContent? content, string errorReason)
        {
            var queueItem = NotificationLogQueueItem.Failure(message, content, errorReason);
            var queueLength = await _redisDb.ListLeftPushAsync("log_queue", JsonSerializer.Serialize(queueItem));

            await _statsRepository.IncrementAsync(
                message.SubscriberId,
                NotificationChannel.From(message.Channel),
                Domain.Events.EventType.From(message.EventType),
                false);

            _logger.LogInformation("[QUEUE] Failure log queued. Recipient={Recipient}, QueueLength={QueueLength}", message.Recipient, queueLength);
            _logger.LogWarning("[FAILED] Notification failed for {Recipient}: {Reason}", message.Recipient, errorReason);
        }

        public async Task LogRejectedAsync(ProcessNotificationMessage message, NotificationContent? content, string errorReason)
        {
            var queueItem = NotificationLogQueueItem.Rejected(message, content, errorReason);
            var queueLength = await _redisDb.ListLeftPushAsync("log_queue", JsonSerializer.Serialize(queueItem));

            await _statsRepository.IncrementAsync(
                message.SubscriberId,
                NotificationChannel.From(message.Channel),
                Domain.Events.EventType.From(message.EventType),
                false);
            
            _logger.LogInformation("[QUEUE] Rejected log queued. Recipient={Recipient}, QueueLength={QueueLength}", message.Recipient, queueLength);
            _logger.LogWarning("[REJECTED] Notification rejected for {Recipient}: {Reason}", message.Recipient, errorReason);
        }
    }
}
