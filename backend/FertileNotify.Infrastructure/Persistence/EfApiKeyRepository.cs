namespace FertileNotify.Infrastructure.Persistence
{
    public class EfApiKeyRepository : IApiKeyRepository
    {
        private readonly ApplicationDbContext _context;

        public EfApiKeyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(ApiKey apiKey)
        {
            var exists = await _context.ApiKeys.AnyAsync(k => k.Id == apiKey.Id);
            if (!exists)
                _context.ApiKeys.Add(apiKey);
            else
                _context.ApiKeys.Update(apiKey);
            await _context.SaveChangesAsync();
        }

        public async Task<ApiKey?> GetByKeyHashAsync(string keyHash)
            => await _context.ApiKeys.AsNoTracking().FirstOrDefaultAsync(k => k.KeyHash == keyHash);

        public async Task<ApiKey?> GetByIdAsync(Guid apiKeyId)
            => await _context.ApiKeys.FirstOrDefaultAsync(k => k.Id == apiKeyId);

        public async Task<List<ApiKey>> GetBySubscriberIdAsync(Guid subscriberId)
            => await _context.ApiKeys
                .AsNoTracking()
                .Where(k => k.SubscriberId == subscriberId)
                .OrderByDescending(k => k.CreatedAt)
                .ToListAsync();

        public async Task DeleteByIdAsync(Guid subscriberId, Guid apiKeyId)
        {
            var key = await _context.ApiKeys
                .FirstOrDefaultAsync(k => k.SubscriberId == subscriberId && k.Id == apiKeyId);

            if (key == null)
                return;

            _context.ApiKeys.Remove(key);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBySubscriberIdAsync(Guid subscriberId)
        {
            var keys = await _context.ApiKeys
                .Where(k => k.SubscriberId == subscriberId)
                .ToListAsync();

            if (keys.Count == 0)
                return;

            _context.ApiKeys.RemoveRange(keys);
            await _context.SaveChangesAsync();
        }
    }
}
