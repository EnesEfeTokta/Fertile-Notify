namespace FertileNotify.Application.Interfaces.Payment
{
    public interface IPaymentService
    {
        Task<string> CreateCustomerAsync(Subscriber subscriber);
        Task<string> CreatePaymentIntentAsync(long amount, string customerId);
        Task<ExtraCreditPaymentIntentDto> CreateExtraCreditPaymentIntentAsync(Subscriber subscriber, int credits);
    }
}
