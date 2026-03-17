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
            => await _context.Set<NotificationLog>()
                .AsNoTracking()
                .Where(l => l.SubscriberId == subscriberId)
                .OrderByDescending(l => l.CreatedAt)
                .Take(count)
                .ToListAsync();

        public async Task<List<NotificationLog>> GetLogsForStatsAsync(Guid subscriberId, DateTime startDate)
            => await _context.NotificationLogs
                .Where(l => l.SubscriberId == subscriberId && l.CreatedAt >= startDate)
                .AsNoTracking()
                .ToListAsync();

        public async Task<List<NotificationLog>> GetLogsForAnonymizationAsync(DateTime cutOffDate)
            => await _context.Set<NotificationLog>()
                .Where(l => l.CreatedAt <= cutOffDate)
                .ToListAsync();

        public async Task UpdateRangeAsync(IEnumerable<NotificationLog> logs)
        {
            _context.Set<NotificationLog>().UpdateRange(logs);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteLogsOlderThanAsync(DateTime cutOffDate)
        {
            var logsToDelete = _context.Set<NotificationLog>()
                .Where(l => l.CreatedAt <= cutOffDate);
            
            _context.Set<NotificationLog>().RemoveRange(logsToDelete);
            await _context.SaveChangesAsync();
        }
    }
}