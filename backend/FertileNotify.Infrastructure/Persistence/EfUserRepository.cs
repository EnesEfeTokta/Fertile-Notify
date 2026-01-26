using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FertileNotify.Infrastructure.Persistence
{
    public class EfUserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public EfUserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(User user)
        {
            if (!_context.Users.Local.Any(u => u.Id == user.Id))
            {
                var exists = await _context.Users.AnyAsync(u => u.Id == user.Id);
                if (!exists)
                    await _context.Users.AddAsync(user);
                else
                    _context.Users.Update(user);
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _context.Users.AnyAsync(u => u.Id == id);
        }
    }
}