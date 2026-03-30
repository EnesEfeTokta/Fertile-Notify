namespace FertileNotify.Application.UseCases.DeleteAccount
{
    public class DeleteAccountHandler: IRequestHandler<DeleteAccountCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ILogger<DeleteAccountHandler> _logger;

        public DeleteAccountHandler(
            ISubscriberRepository subscriberRepository,
            ILogger<DeleteAccountHandler> logger)
        {
            _subscriberRepository = subscriberRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteAccountCommand command, CancellationToken ct)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            await _subscriberRepository.DeleteAsync(subscriber.Id);

            _logger.LogInformation("Subscriber with ID {SubscriberId} has been deleted.", command.SubscriberId);
            return Unit.Value;
        }
    }
}
