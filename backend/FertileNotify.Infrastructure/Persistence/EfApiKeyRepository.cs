using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using Microsoft.EntityFrameworkCore;

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
            return _context.SaveChangesAsync();
        }

        public async Task<ApiKey?> GetByKeyHashAsync(string keyHash)
            => await _context.ApiKeys.AsNoTracking().FirstOrDefaultAsync(k => k.KeyHash == keyHash);

        public Task<List<ApiKey>> GetBySubscriberIdAsync(Guid subscriberId)
            => _context.ApiKeys.AsNoTracking().Where(k => k.SubscriberId == subscriberId).OrderByDescending(k => k.CreatedAt).ToListAsync();
    }
}
