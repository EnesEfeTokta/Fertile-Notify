namespace FertileNotify.Infrastructure.Persistence
{
    public class EfSystemNotificationRepository : ISystemNotificationRepository
    {
        private readonly ApplicationDbContext _context;

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
            => await _context.SystemNotifications
                .Where(n => n.IsRead == isRead)
                .AsNoTracking()
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

        public async Task<SystemNotification?> GetByIdAsync(Guid notificationId)
            => await _context.SystemNotifications
                .AsNoTracking()
                .FirstOrDefaultAsync(n => n.Id == notificationId);

        public async Task<List<SystemNotification>> GetBySubscriberIdAsync(Guid subscriberId, bool? isRead = null)
        {
            var query = _context.SystemNotifications
                .AsNoTracking()
                .Where(n => n.SubscriberId == subscriberId);

            if (isRead.HasValue)
            {
                query = query.Where(n => n.IsRead == isRead.Value);
            }

            return await query
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _context.SystemNotifications.FindAsync(notificationId);
            if (notification != null)
            {
                if (!notification.IsRead)
                {
                    notification.MarkAsRead();
                    _context.SystemNotifications.Update(notification);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}
