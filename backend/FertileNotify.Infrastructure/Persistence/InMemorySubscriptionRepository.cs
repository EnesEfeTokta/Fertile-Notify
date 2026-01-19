using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;

namespace FertileNotify.Infrastructure.Persistence
{
    public class InMemorySubscriptionRepository : ISubscriptionRepository
    {
        private readonly Dictionary<Guid, Subscription> _storage = new();

        public Task<Subscription?> GetByIdAsync(Guid userId)
        {
            var subscription = _storage.Values
                .FirstOrDefault(s => s.UserId == userId);

            return Task.FromResult(subscription);
        }

        public Task SaveAsync(Subscription subscription)
        {
            _storage[subscription.Id] = subscription;
            return Task.CompletedTask;
        }
    }
}
