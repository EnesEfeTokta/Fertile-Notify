using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Interfaces
{
    public interface ISubscriberRepository
    {
        Task SaveAsync(Subscriber subscriber);
        Task<Subscriber?> GetByIdAsync(Guid id);
        Task<List<Guid>> GetExistingIdsAsync(List<Guid> ids);
        Task<Subscriber?> GetByEmailAsync(EmailAddress email);
        Task<bool> ExistsAsync(Guid id);
    }
}
