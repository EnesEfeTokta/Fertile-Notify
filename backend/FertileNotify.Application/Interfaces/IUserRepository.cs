using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Interfaces
{
    public interface IUserRepository
    {
        Task SaveAsync(User user);
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByEmailAsync(EmailAddress email);
        Task<bool> ExistsAsync(Guid id);
    }
}
