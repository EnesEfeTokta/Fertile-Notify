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

        public async Task<NotificationTemplate?> GetByEventTypeAsync(EventType eventType)
        {
            var allTemplates = await _context.NotificationTemplates.ToListAsync();
            return allTemplates.FirstOrDefault(t => t.EventType.Equals(eventType));
        }
    }
}