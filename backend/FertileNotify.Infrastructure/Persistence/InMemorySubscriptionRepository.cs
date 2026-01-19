using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;

namespace FertileNotify.Infrastructure.Persistence
{
    public class InMemorySubscriptionRepository : ISubscriptionRepository
    {
        private static readonly Dictionary<Guid, Subscription> _subscriptions = new();

        public Task SaveAsync(Guid userId, Subscription subscription)
        {
            _subscriptions[userId] = subscription;
            return Task.CompletedTask;
        }

        public Task<Subscription?> GetByUserIdAsync(Guid userId)
        {
            _subscriptions.TryGetValue(userId, out var sub);
            return Task.FromResult(sub);
        }
    }
}
