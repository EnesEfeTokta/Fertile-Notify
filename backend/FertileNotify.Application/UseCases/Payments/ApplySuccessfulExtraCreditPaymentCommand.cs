namespace FertileNotify.Application.UseCases.Payments
{
    public class ApplySuccessfulExtraCreditPaymentCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public int Credits { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;
        public long AmountInCents { get; set; }
    }
}
