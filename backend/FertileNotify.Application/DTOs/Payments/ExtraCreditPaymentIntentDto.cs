namespace FertileNotify.Application.DTOs.Payments
{
    public class ExtraCreditPaymentIntentDto
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public int Credits { get; set; }
        public long AmountInCents { get; set; }
        public string Currency { get; set; } = "usd";
    }
}
