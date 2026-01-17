using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;

namespace FertileNotify.Infrastructure.Persistence
{
    public class InMemorySubscriptionRepository : ISubscriptionRepository
    {
        private readonly Dictionary<Guid, Subscription> _subscriptions = new();

        public Task<Subscription?> GetByIdAsync(Guid userId)
        {
            _subscriptions.TryGetValue(userId, out var subscription);
            return Task.FromResult(subscription);
        }

        public Task SaveAsync(Subscription subscription)
        {
            _subscriptions[subscription.Id] = subscription;
            return Task.CompletedTask;
        }
    }
}
