namespace FertileNotify.Infrastructure.Persistence
{
    public class EfPaymentLogRepository : IPaymentLogRepository
    {
        private readonly ApplicationDbContext _context;

        public EfPaymentLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddPaymentLogAsync(Guid subscriberId, string intentId, decimal amount, string status)
        {
            var paymentLog = new PaymentLog(subscriberId, intentId, amount, status);
            await _context.PaymentLogs.AddAsync(paymentLog);
            await _context.SaveChangesAsync();
        }

        public async Task<PaymentLog?> GetPaymentLogByIntentIdAsync(string intentId)
        {
            return await _context.PaymentLogs
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.StripePaymentIntentId == intentId);
        }

        public async Task<List<PaymentLog>> GetPaymentLogsBySubscriberIdAsync(Guid subscriberId)
        {
            return await _context.PaymentLogs
                .AsNoTracking()
                .Where(l => l.SubscriberId == subscriberId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }
    }
}
