namespace FertileNotify.Infrastructure.Persistence
{
    public class EfSystemNotificationRepository : ISystemNotificationRepository
    {
        ApplicationDbContext _context;

        public EfSystemNotificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(SystemNotification notification)
        {
            await _context.SystemNotifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid notificationId)
            => await _context.SystemNotifications.Where(n => n.Id == notificationId).ExecuteDeleteAsync();

        public async Task<List<SystemNotification>> GetAllByIsReadAsync(bool isRead = true)
            => await _context.SystemNotifications.Where(n => n.IsRead == isRead).AsNoTracking().ToListAsync();

        public async Task<List<SystemNotification>> GetBySubscriberIdAsync(Guid subscriberId)
            => await _context.SystemNotifications.Where(n => n.SubscriberId == subscriberId).AsNoTracking().ToListAsync();

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.SystemNotifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.MarkAsRead();
                _context.SystemNotifications.Update(notification);
                await _context.SaveChangesAsync();
            }
        }
    }
}
