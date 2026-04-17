namespace FertileNotify.Application.UseCases.Payments
{
    public class ApplySuccessfulExtraCreditPaymentHandler : IRequestHandler<ApplySuccessfulExtraCreditPaymentCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly IPaymentLogRepository _paymentLogRepository;

        public ApplySuccessfulExtraCreditPaymentHandler(
            ISubscriberRepository subscriberRepository,
            IPaymentLogRepository paymentLogRepository)
        {
            _subscriberRepository = subscriberRepository;
            _paymentLogRepository = paymentLogRepository;
        }

        public async Task<Unit> Handle(ApplySuccessfulExtraCreditPaymentCommand command, CancellationToken cancellationToken)
        {
            if (command.Credits <= 0)
                throw new BusinessRuleException("Credits must be greater than zero.", "PAY_1002");

            if (string.IsNullOrWhiteSpace(command.PaymentIntentId))
                throw new BusinessRuleException("Payment intent id is required.", "PAY_1004");

            // Stripe can retry the same webhook event, so guard by intent id.
            var existingLog = await _paymentLogRepository.GetPaymentLogByIntentIdAsync(command.PaymentIntentId);
            if (existingLog is not null)
                return Unit.Value;

            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            subscriber.AddCredits(command.Credits);
            await _subscriberRepository.SaveAsync(subscriber);

            var amount = command.AmountInCents / 100m;
            await _paymentLogRepository.AddPaymentLogAsync(
                command.SubscriberId,
                command.PaymentIntentId,
                amount,
                "succeeded");

            return Unit.Value;
        }
    }
}
