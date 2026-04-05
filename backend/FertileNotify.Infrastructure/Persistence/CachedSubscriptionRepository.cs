using System.Text.Json;
using FertileNotify.Domain.Enums;
using Microsoft.Extensions.Caching.Distributed;

namespace FertileNotify.Infrastructure.Persistence
{
    public class CachedSubscriptionRepository : ISubscriptionRepository
    {
        private readonly ISubscriptionRepository _decorated;
        private readonly IDistributedCache _cache;
        private readonly ILogger<CachedSubscriptionRepository> _logger;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(15);

        public CachedSubscriptionRepository(
            ISubscriptionRepository decorated,
            IDistributedCache cache,
            ILogger<CachedSubscriptionRepository> logger)
        {
            _decorated = decorated;
            _cache = cache;
            _logger = logger;
        }

        public async Task<Subscription?> GetBySubscriberIdAsync(Guid subscriberId)
        {
            string cacheKey = $"sub:{subscriberId}";

            var cached = await _cache.GetStringAsync(cacheKey);
            if (cached != null)
            {
                try
                {
                    var cachedSubscription = JsonSerializer.Deserialize<SubscriptionCacheDto>(cached)?.ToDomain();
                    if (cachedSubscription != null)
                    {
                        return cachedSubscription;
                    }
                }
                catch (NotSupportedException ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize cached subscription for subscriber {SubscriberId}. Falling back to DB.", subscriberId);
                    await _cache.RemoveAsync(cacheKey);
                }
            }

            var subscription = await _decorated.GetBySubscriberIdAsync(subscriberId);

            if (subscription != null)
            {
                try
                {
                    var cacheDto = SubscriptionCacheDto.FromDomain(subscription);
                    await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(cacheDto),
                        new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheDuration });
                }
                catch (NotSupportedException ex)
                {
                    _logger.LogWarning(ex, "Failed to serialize subscription for subscriber {SubscriberId}. Skipping cache write.", subscriberId);
                }
            }

            return subscription;
        }

        public async Task SaveAsync(Guid userId, Subscription subscription)
        {
            await _decorated.SaveAsync(userId, subscription);
            await _cache.RemoveAsync($"sub:{userId}");
        }

        public async Task DeleteBySubscriberIdAsync(Guid subscriberId)
        {
            await _decorated.DeleteBySubscriberIdAsync(subscriberId);
            await _cache.RemoveAsync($"sub:{subscriberId}");
        }

        private sealed class SubscriptionCacheDto
        {
            public Guid Id { get; init; }
            public Guid SubscriberId { get; init; }
            public SubscriptionPlan Plan { get; init; }
            public int MonthlyLimit { get; init; }
            public int UsedThisMonth { get; init; }
            public DateTime ExpiresAt { get; init; }
            public string[] AllowedEvents { get; init; } = [];

            public static SubscriptionCacheDto FromDomain(Subscription subscription)
            {
                return new SubscriptionCacheDto
                {
                    Id = subscription.Id,
                    SubscriberId = subscription.SubscriberId,
                    Plan = subscription.Plan,
                    MonthlyLimit = subscription.MonthlyLimit,
                    UsedThisMonth = subscription.UsedThisMonth,
                    ExpiresAt = subscription.ExpiresAt,
                    AllowedEvents = subscription.AllowedEvents.Select(x => x.Name).ToArray()
                };
            }

            public Subscription ToDomain()
            {
                var allowedEventTypes = AllowedEvents.Select(EventType.From);
                return Subscription.Restore(
                    Id,
                    SubscriberId,
                    Plan,
                    MonthlyLimit,
                    UsedThisMonth,
                    ExpiresAt,
                    allowedEventTypes);
            }
        }
    }
}
