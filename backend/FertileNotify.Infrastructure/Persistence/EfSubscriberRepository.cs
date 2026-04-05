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
            var trackedEntry = _context.ChangeTracker
                .Entries<Subscriber>()
                .FirstOrDefault(e => e.Entity.Id == subscriber.Id);

            if (trackedEntry != null)
            {
                await _context.SaveChangesAsync();
                return;
            }

            var exists = await _context.Subscribers.AnyAsync(u => u.Id == subscriber.Id);
            if (!exists)
                await _context.Subscribers.AddAsync(subscriber);
            else
                _context.Subscribers.Update(subscriber);

            await _context.SaveChangesAsync();
        }

        public async Task<Subscriber?> GetByIdAsync(Guid id)
            => await _context.Subscribers.FirstOrDefaultAsync(u => u.Id == id);

        public async Task<List<Guid>> GetExistingIdsAsync(List<Guid> ids)
            => await _context.Subscribers.AsNoTracking().Where(u => ids.Contains(u.Id)).Select(u => u.Id).ToListAsync();

        public async Task<Subscriber?> GetByEmailAsync(EmailAddress email)
            => await _context.Subscribers.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<Subscriber?> GetByPhoneNumberAsync(PhoneNumber phoneNumber)
            => await _context.Subscribers.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);

        public async Task<Subscriber?> GetByRefreshTokenAsync(string refreshToken)
            => await _context.Subscribers.FirstOrDefaultAsync(u => u.RefreshToken!.Token == refreshToken);

        public async Task<bool> ExistsAsync(Guid id)
            => await _context.Subscribers.AnyAsync(u => u.Id == id);

        public async Task DeleteAsync(Guid id)
        {
            var subscriber = await _context.Subscribers.FindAsync(id);
            if (subscriber is not null)
            {
                _context.Subscribers.Remove(subscriber);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteBySubscriberIdAsync(Guid subscriberId)
            => await _context.Subscribers.Where(s => s.Id == subscriberId).ExecuteDeleteAsync();
    }
}