using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FertileNotify.Infrastructure.Persistence
{
    public class EfSubscriptionRepository : ISubscriptionRepository
    {
        private readonly ApplicationDbContext _context;

        public EfSubscriptionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(Guid userId, Subscription subscription)
        {
            if (!_context.Subscriptions.Local.Any(s => s.Id == subscription.Id))
            {
                 var exists = await _context.Subscriptions.AnyAsync(s => s.Id == subscription.Id);
                 if(!exists) await _context.Subscriptions.AddAsync(subscription);
                 else _context.Subscriptions.Update(subscription);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<Subscription?> GetByUserIdAsync(Guid userId)
        {
            return await _context.Subscriptions.FirstOrDefaultAsync(s => s.UserId == userId);
        }
    }
}