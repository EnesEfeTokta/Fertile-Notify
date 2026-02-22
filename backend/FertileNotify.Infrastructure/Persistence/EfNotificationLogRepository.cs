using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FertileNotify.Infrastructure.Persistence
{
    public class EfNotificationLogRepository : INotificationLogRepository
    {
        private readonly ApplicationDbContext _context;

        public EfNotificationLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(NotificationLog log)
        {
            await _context.Set<NotificationLog>().AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationLog>> GetLatestBySubscriberIdAsync(Guid subscriberId, int count)
        {
            return await _context.Set<NotificationLog>()
                .Where(l => l.SubscriberId == subscriberId)
                .OrderByDescending(l => l.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<NotificationLog>> GetLogsForStatsAsync(Guid subscriberId, DateTime startDate)
        {
            return await _context.NotificationLogs
                .Where(l => l.SubscriberId == subscriberId && l.CreatedAt >= startDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}