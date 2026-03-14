using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FertileNotify.Infrastructure.Persistence
{
    public class EfBlacklistRepository : IBlacklistRepository
    {
        private readonly ApplicationDbContext _context;

        public EfBlacklistRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddOrUpdateAsync(ForbiddenRecipient recipient)
        {
            var existing = await _context.ForbiddenRecipients.FirstOrDefaultAsync(b => 
                b.UnwantedSubscriber == recipient.UnwantedSubscriber && 
                b.RecipientAddress == recipient.RecipientAddress);

            if (existing == null) 
                await _context.ForbiddenRecipients.AddAsync(recipient);
            else 
            {
                existing.UpdateChannels(recipient.UnwantedChannels);
                if (recipient.IsActive) existing.Activate(); else existing.Deactivate();
                _context.ForbiddenRecipients.Update(existing);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsBlacklistedAsync(Guid unwantedSubscriber, string address)
            => await _context.ForbiddenRecipients.AsNoTracking()
                .AnyAsync(b => b.UnwantedSubscriber == unwantedSubscriber && b.RecipientAddress == address && b.IsActive);

        public async Task<List<ForbiddenRecipient>> GetForRecipientsAsync(Guid subscriberId, List<string> addresses)
            => await _context.ForbiddenRecipients.AsNoTracking()
                .Where(b => b.UnwantedSubscriber == subscriberId && addresses.Contains(b.RecipientAddress) && b.IsActive)
                .ToListAsync();

        public async Task<List<ForbiddenRecipient>> GetAllBySubscriberAsync(Guid subscriberId)
            => await _context.ForbiddenRecipients.AsNoTracking()
                .Where(b => b.UnwantedSubscriber == subscriberId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync();

        public async Task<ForbiddenRecipient?> GetByIdAsync(Guid id)
            => await _context.ForbiddenRecipients.FirstOrDefaultAsync(b => b.Id == id);

        public async Task DeleteAsync(Guid id)
        {
            var entry = await _context.ForbiddenRecipients.FindAsync(id);
            if (entry != null)
            {
                _context.ForbiddenRecipients.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }
    }
}
