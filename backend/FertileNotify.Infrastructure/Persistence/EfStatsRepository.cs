using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FertileNotify.Infrastructure.Persistence
{
    public class EfStatsRepository : IStatsRepository
    {
        private readonly ApplicationDbContext _context;

        public EfStatsRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task IncrementAsync(Guid subscriberId, NotificationChannel channel, EventType eventType, bool isSuccess)
        {
            var today = DateTime.UtcNow.Date;

            var stat = await _context.DailyStats.FirstOrDefaultAsync(x =>
                x.SubscriberId == subscriberId &&
                x.Date == today &&
                x.Channel == channel &&
                x.EventType == eventType);

            if (stat == null)
            {
                stat = new SubscriberDailyStats(subscriberId, channel, eventType);
                await _context.DailyStats.AddAsync(stat);
            }

            if (isSuccess) stat.IncreaseSuccess();
            else stat.IncreaseFailure();

            await _context.SaveChangesAsync();
        }

        public async Task<List<SubscriberDailyStats>> GetStatsAsync(Guid subscriberId, DateTime startDate, DateTime endDate)
            => await _context.DailyStats.Where(x => x.SubscriberId == subscriberId && x.Date >= startDate && x.Date <= endDate).ToListAsync();
    }
}
