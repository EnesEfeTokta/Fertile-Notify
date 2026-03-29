namespace FertileNotify.Application.UseCases.DeleteAccount
{
    public class DeleteAccountHandler: IRequestHandler<DeleteAccountCommand, Unit>
    {
        private readonly ILogger<DeleteAccountHandler> _logger;

        public DeleteAccountHandler(ILogger<DeleteAccountHandler> logger)
        {
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteAccountCommand command, CancellationToken ct)
        {
            _logger.LogInformation("Subscriber with ID {SubscriberId} has been deleted.", command.SubscriberId);
            return Unit.Value;
        }
    }
}

