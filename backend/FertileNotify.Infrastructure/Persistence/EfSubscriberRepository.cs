using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FertileNotify.Infrastructure.Persistence
{
    public class EfSubscriberRepository : ISubscriberRepository
    {
        private readonly ApplicationDbContext _context;

        public EfSubscriberRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(Subscriber subscriber)
        {
            if (!_context.Subscribers.Local.Any(u => u.Id == subscriber.Id || u.CompanyName == subscriber.CompanyName))
            {
                var exists = await _context.Subscribers.AnyAsync(u => u.Id == subscriber.Id);
                if (!exists)
                    await _context.Subscribers.AddAsync(subscriber);
                else
                    _context.Subscribers.Update(subscriber);
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task<Subscriber?> GetByIdAsync(Guid id)
            => await _context.Subscribers.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<List<Guid>> GetExistingIdsAsync(List<Guid> ids)
            => await _context.Subscribers.Where(u => ids.Contains(u.Id)).Select(u => u.Id).ToListAsync();

        public async Task<Subscriber?> GetByEmailAsync(EmailAddress email)
            => await _context.Subscribers.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<bool> ExistsAsync(Guid id)
            => await _context.Subscribers.AnyAsync(u => u.Id == id);
    }
}