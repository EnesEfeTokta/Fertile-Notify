namespace FertileNotify.Infrastructure.Persistence
{
    public class EfINotificationComplaintRepository : INotificationComplaintRepository
    {
        private readonly ApplicationDbContext _context;

        public EfINotificationComplaintRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<NotificationComplaint?> GetByIdAsync(Guid id)
            => await _context.NotificationComplaints.AsNoTracking().FirstOrDefaultAsync(nc => nc.Id == id);

        public async Task<List<NotificationComplaint>> GetComplaintsBySubscriberIdAsync(Guid subscriberId)
            => await _context.NotificationComplaints.AsNoTracking().Where(nc => nc.SubscriberId == subscriberId).OrderByDescending(nc => nc.CreatedAt).ToListAsync();

        public async Task SaveAsync(NotificationComplaint complaint)
        {
            var exists = await _context.NotificationComplaints.AnyAsync(nc => nc.Id == complaint.Id);
            if (!exists)
                _context.NotificationComplaints.Add(complaint);
            else
                _context.NotificationComplaints.Update(complaint);
            await _context.SaveChangesAsync();
        }
    }
}
