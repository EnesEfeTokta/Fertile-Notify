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
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t =>
                        t.SubscriberId == subscriberId && t.EventType == eventType && t.Channel == channel);

                if (customTemplate != null) return customTemplate;
            }

            // Bug fix: include channel filter on global fallback to avoid wrong-channel template
            return await _context.NotificationTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.SubscriberId == null && t.EventType == eventType && t.Channel == channel);
        }

        public async Task<NotificationTemplate?> GetGlobalTemplateAsync(EventType eventType, NotificationChannel channel)
            => await _context.NotificationTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.SubscriberId == null && t.EventType == eventType && t.Channel == channel);

        public async Task<NotificationTemplate?> GetCustomTemplateAsync(
            EventType eventType, NotificationChannel channel, Guid subscriberId)
            => await _context.NotificationTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.SubscriberId == subscriberId && t.EventType == eventType && t.Channel == channel);

        public async Task<List<NotificationTemplate>> GetAllTemplatesAsync(Guid subscriberId)
        {
            // Single query instead of two separate roundtrips
            return await _context.NotificationTemplates
                .AsNoTracking()
                .Where(t => t.SubscriberId == null || t.SubscriberId == subscriberId)
                .ToListAsync();
        }
    }
}