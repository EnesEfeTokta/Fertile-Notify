using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface IUserRepository
    {
        Task SaveAsync(User user);
        Task<User?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
