namespace FertileNotify.Application.DTOs.Payments
{
    public class PaymentLogDto
    {
        public Guid Id { get; set; }
        public string StripePaymentIntentId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}