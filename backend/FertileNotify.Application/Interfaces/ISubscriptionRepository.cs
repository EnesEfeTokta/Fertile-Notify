using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task SaveAsync(Guid userId, Subscription subscription);
        Task<Subscription?> GetByUserIdAsync(Guid userId);
    }
}
