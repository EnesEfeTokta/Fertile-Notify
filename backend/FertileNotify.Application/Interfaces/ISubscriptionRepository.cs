using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task SaveAsync(Guid subscriberId, Subscription subscription);
        Task<Subscription?> GetBySubscriberIdAsync(Guid subscriberId);
    }
}
