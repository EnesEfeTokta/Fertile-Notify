namespace FertileNotify.Application.UseCases.UpdatePassword
{
    public class UpdatePasswordHandler : IRequestHandler<UpdatePasswordCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ILogger<UpdatePasswordHandler> _logger;

        public UpdatePasswordHandler(
            ISubscriberRepository subscriberRepository,
            ILogger<UpdatePasswordHandler> logger)
        {
            _subscriberRepository = subscriberRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            subscriber.UpdatePassword(command.CurrentPassword, Password.Create(command.NewPassword));
            await _subscriberRepository.SaveAsync(subscriber);

            _logger.LogInformation("Password updated for Subscriber {SubscriberId}.", command.SubscriberId);
            return Unit.Value;
        }
    }
}
