using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;

namespace FertileNotify.Infrastructure.Persistence
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly Dictionary<Guid, User> _users = new();

        public Task SaveAsync(User user)
        {
            _users[user.Id] = user;
            return Task.CompletedTask;
        }

        public Task<User?> GetByIdAsync(Guid id)
        {
            _users.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }
    }
}
