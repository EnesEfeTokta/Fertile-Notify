using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using Microsoft.EntityFrameworkCore;

namespace FertileNotify.Infrastructure.Persistence
{
    public class EfTemplateRepository : ITemplateRepository
    {
        private readonly ApplicationDbContext _context;

        public EfTemplateRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(NotificationTemplate template)
        {
            await _context.NotificationTemplates.AddAsync(template);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        public async Task<NotificationTemplate?> GetTemplateAsync(EventType eventType, Guid? subscriberId)
        {
            if (subscriberId.HasValue)
            {
                var customTemplate = await _context.NotificationTemplates
                    .FirstOrDefaultAsync(t => 
                        t.SubscriberId == subscriberId 
                        && t.EventType == eventType);

                if (customTemplate != null) return customTemplate;
            }

            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.SubscriberId == null && t.EventType == eventType);
        }

        public async Task<NotificationTemplate?> GetGlobalTemplateAsync(EventType eventType)
            => await _context.NotificationTemplates.FirstOrDefaultAsync(t =>
                    t.SubscriberId == null && t.EventType == eventType);

        public async Task<NotificationTemplate?> GetCustomTemplateAsync(EventType eventType, Guid subscriberId)
            => await _context.NotificationTemplates.FirstOrDefaultAsync(t =>
                    t.SubscriberId == subscriberId && t.EventType == eventType);
    }
}