using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
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

        public async Task<NotificationTemplate?> GetTemplateAsync(EventType eventType, NotificationChannel channel, Guid? subscriberId)
        {
            if (subscriberId.HasValue)
            {
                var customTemplate = await _context.NotificationTemplates
                    .FirstOrDefaultAsync(t => 
                        t.SubscriberId == subscriberId && t.EventType == eventType && t.Channel == channel);

                if (customTemplate != null) return customTemplate;
            }

            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.SubscriberId == null && t.EventType == eventType);
        }

        public async Task<NotificationTemplate?> GetGlobalTemplateAsync(EventType eventType, NotificationChannel channel)
            => await _context.NotificationTemplates.FirstOrDefaultAsync(t =>
                    t.SubscriberId == null && t.EventType == eventType && t.Channel == channel);

        public async Task<NotificationTemplate?> GetCustomTemplateAsync(
            EventType eventType, NotificationChannel channel, Guid subscriberId)
            => await _context.NotificationTemplates.FirstOrDefaultAsync(t =>
                    t.SubscriberId == subscriberId && t.EventType == eventType && t.Channel == channel);

        public async Task<List<NotificationTemplate>> GetAllTemplatesAsync(Guid subscriberId)
        {
            var globalTemplates = await _context.NotificationTemplates
                .Where(t => t.SubscriberId == null)
                .ToListAsync();

            var customTemplates = await _context.NotificationTemplates
                .Where(t => t.SubscriberId == subscriberId)
                .ToListAsync();

            return globalTemplates.Concat(customTemplates).ToList();
        }
    }
}