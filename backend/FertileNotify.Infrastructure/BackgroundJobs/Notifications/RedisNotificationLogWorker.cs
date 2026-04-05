using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace FertileNotify.Infrastructure.BackgroundJobs.Notifications
{
    public class RedisNotificationLogWorker : BackgroundService
    {
        private static readonly TimeSpan MinQueueWait = TimeSpan.FromMinutes(1);
        private readonly IServiceProvider _serviceProvider;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisNotificationLogWorker> _logger;

        public RedisNotificationLogWorker(
            IServiceProvider serviceProvider, 
            IConnectionMultiplexer redis,
            ILogger<RedisNotificationLogWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _redis = redis;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = _redis.GetDatabase();
            _logger.LogInformation("RedisNotificationLogWorker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var logs = new List<NotificationLog>();
                var rawQueueItems = new List<string>();
                var deferredCount = 0;
                var processBefore = DateTime.UtcNow - MinQueueWait;
                var initialQueueLength = await db.ListLengthAsync("log_queue");

                for (long i = 0; i < initialQueueLength; i++)
                {
                    var item = await db.ListRightPopAsync("log_queue");
                    if (item.HasValue)
                    {
                        var rawItem = (string)item!;
                        rawQueueItems.Add(rawItem);

                        try
                        {
                            var queueItem = JsonSerializer.Deserialize<NotificationLogQueueItem>(rawItem);
                            if (queueItem == null)
                            {
                                _logger.LogWarning("Skipping null queue item from Redis key {Key}.", "log_queue");
                                continue;
                            }

                            if (queueItem.QueuedAtUtc > processBefore)
                            {
                                deferredCount++;
                                await db.ListLeftPushAsync("log_queue", rawItem);
                                continue;
                            }

                            logs.Add(queueItem.ToDomain());
                            rawQueueItems.Add(rawItem);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Invalid queue payload. Item skipped.");
                        }
                    }
                }

                if (logs.Any())
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var notificationLogRepository = scope.ServiceProvider.GetRequiredService<INotificationLogRepository>();
                        
                        await notificationLogRepository.AddRangeAsync(logs);
                        
                        _logger.LogInformation("[LOG WORKER] {Count} logs saved to database.", logs.Count);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to save logs to database.");

                        foreach (var raw in rawQueueItems)
                        {
                            await db.ListLeftPushAsync("log_queue", raw);
                        }
                    }
                }
                else
                {
                    _logger.LogDebug("[LOG WORKER] No ready logs found. Deferred={DeferredCount}, Next flush in 1 minute.", deferredCount);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }
}