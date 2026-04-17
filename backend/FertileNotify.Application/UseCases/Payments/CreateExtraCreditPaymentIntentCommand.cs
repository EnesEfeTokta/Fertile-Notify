namespace FertileNotify.Application.UseCases.Payments
{
    public class CreateExtraCreditPaymentIntentCommand : IRequest<ExtraCreditPaymentIntentDto>
    {
        public Guid SubscriberId { get; set; }
        public int Credits { get; set; }
    }
}
