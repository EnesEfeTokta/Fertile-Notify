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
            var exists = await _context.Subscriptions.AnyAsync(s => s.Id == subscription.Id);
            if (!exists) await _context.Subscriptions.AddAsync(subscription);
            else _context.Subscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task<Subscription?> GetBySubscriberIdAsync(Guid subscriberId)
            => await _context.Subscriptions.AsNoTracking().FirstOrDefaultAsync(s => s.SubscriberId == subscriberId);
    }
}