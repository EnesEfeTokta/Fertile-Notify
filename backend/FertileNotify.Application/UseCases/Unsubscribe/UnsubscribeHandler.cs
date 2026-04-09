namespace FertileNotify.Application.UseCases.Unsubscribe
{
    public class UnsubscribeHandler : IRequestHandler<UnsubscribeCommand, Unit>
    {
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly ISecurityService _securityService;
        private readonly ILogger<UnsubscribeHandler> _logger;

        public UnsubscribeHandler(
            IBlacklistRepository blacklistRepository,
            ISecurityService securityService,
            ILogger<UnsubscribeHandler> logger)
        {
            _blacklistRepository = blacklistRepository;
            _securityService = securityService;
            _logger = logger;
        }

        public async Task<Unit> Handle(UnsubscribeCommand command, CancellationToken cancellationToken)
        {
            bool isValid = _securityService.VerifyUnsubscribeToken(command.Recipient, command.SubscriberId, command.Token);

            if (!isValid)
                return Unit.Value;

            var forbiddenRecipient = new ForbiddenRecipient(
                command.SubscriberId,
                command.Recipient,
                command.Channels?.Select(NotificationChannel.From).ToList() ?? new List<NotificationChannel>()
            );
            await _blacklistRepository.AddOrUpdateAsync(forbiddenRecipient);

            _logger.LogInformation("Subscriber {SubscriberId} unsubscribed recipient {Recipient}.",
                command.SubscriberId,
                command.Recipient
            );

            return Unit.Value;
        }
    }
}
