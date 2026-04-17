namespace FertileNotify.Application.UseCases.Payments
{
    public class CreateExtraCreditPaymentIntentHandler : IRequestHandler<CreateExtraCreditPaymentIntentCommand, ExtraCreditPaymentIntentDto>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly IPaymentService _paymentService;

        public CreateExtraCreditPaymentIntentHandler(ISubscriberRepository subscriberRepository, IPaymentService paymentService)
        {
            _subscriberRepository = subscriberRepository;
            _paymentService = paymentService;
        }

        public async Task<ExtraCreditPaymentIntentDto> Handle(CreateExtraCreditPaymentIntentCommand command, CancellationToken cancellationToken)
        {
            if (command.Credits <= 0)
                throw new BusinessRuleException("Credits must be greater than zero.", "PAY_1001");

            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            return await _paymentService.CreateExtraCreditPaymentIntentAsync(subscriber, command.Credits);
        }
    }
}
