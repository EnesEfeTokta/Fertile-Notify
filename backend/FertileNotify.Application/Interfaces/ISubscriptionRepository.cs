using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface ISubscriptionRepository
    {
        Task<Subscription?> GetByIdAsync(Guid userId);
        Task SaveAsync(Subscription subscription);
    }
}