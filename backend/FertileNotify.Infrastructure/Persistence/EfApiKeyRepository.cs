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

        public Task SaveAsync(ApiKey apiKey)
        {
            if (!_context.ApiKeys.Local.Any(k => k.Id == apiKey.Id || k.KeyHash == apiKey.KeyHash))
            {
                var exists = _context.ApiKeys.Any(k => k.Id == apiKey.Id);
                if (!exists)
                    _context.ApiKeys.Add(apiKey);
                else
                    _context.ApiKeys.Update(apiKey);
            }
            return _context.SaveChangesAsync();
        }

        public async Task<ApiKey?> GetByKeyHashAsync(string keyHash)
            => await _context.ApiKeys.FirstOrDefaultAsync(k => k.KeyHash == keyHash);

        public Task<List<ApiKey>> GetBySubscriberIdAsync(Guid subscriberId)
            => _context.ApiKeys.Where(k => k.SubscriberId == subscriberId).OrderByDescending(k => k.CreatedAt).ToListAsync();
    }
}
