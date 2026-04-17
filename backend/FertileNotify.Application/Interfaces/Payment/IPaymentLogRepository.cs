namespace FertileNotify.Application.Interfaces.Payment
{
    public interface IPaymentLogRepository
    {
        Task AddPaymentLogAsync(Guid subscriberId, string intentId, decimal amount, string status);
        Task<PaymentLog?> GetPaymentLogByIntentIdAsync(string intentId);
        Task<List<PaymentLog>> GetPaymentLogsBySubscriberIdAsync(Guid subscriberId);
    }
}
