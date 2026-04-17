namespace FertileNotify.Domain.Entities
{
public class PaymentLog
{
    public Guid Id { get; private set; }
    public Guid SubscriberId { get; private set; }
    public string StripePaymentIntentId { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public string Status { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    private PaymentLog() { }
    public PaymentLog(Guid subscriberId, string intentId, decimal amount, string status)
    {
        Id = Guid.NewGuid();
        SubscriberId = subscriberId;
        StripePaymentIntentId = intentId;
        Amount = amount;
        Status = status;
        CreatedAt = DateTime.UtcNow;
    }
}
}
