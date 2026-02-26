using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace FertileNotify.Infrastructure.Persistence
{
    public class EfSubscriberChannelRepository : ISubscriberChannelRepository
    {
        private readonly ApplicationDbContext _context;

        public EfSubscriberChannelRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SaveAsync(SubscriberChannelSetting setting)
        {
            var existing = await _context.SubscriberChannelSettings
                .FirstOrDefaultAsync(s => s.SubscriberId == setting.SubscriberId && s.Channel == setting.Channel);

            if (existing != null)
            {
                existing.UpdateSettings(setting.Settings as Dictionary<string, string> ?? default!);
                _context.SubscriberChannelSettings.Update(existing);
            }
            else
            {
                await _context.SubscriberChannelSettings.AddAsync(setting);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<SubscriberChannelSetting?> GetSettingAsync(Guid subscriberId, NotificationChannel channel)
        {
            return await _context.SubscriberChannelSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.SubscriberId == subscriberId && s.Channel == channel);
        }
    }
}