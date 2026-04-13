namespace FertileNotify.Application.UseCases.Payments
{
    public class ApplySuccessfulExtraCreditPaymentCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public int Credits { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;
    }

    public class ApplySuccessfulExtraCreditPaymentHandler : IRequestHandler<ApplySuccessfulExtraCreditPaymentCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;

        public ApplySuccessfulExtraCreditPaymentHandler(ISubscriberRepository subscriberRepository)
        {
            _subscriberRepository = subscriberRepository;
        }

        public async Task<Unit> Handle(ApplySuccessfulExtraCreditPaymentCommand command, CancellationToken cancellationToken)
        {
            if (command.Credits <= 0)
                throw new BusinessRuleException("Credits must be greater than zero.", "PAY_1002");

            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            subscriber.AddCredits(command.Credits);
            await _subscriberRepository.SaveAsync(subscriber);

            return Unit.Value;
        }
    }
}
