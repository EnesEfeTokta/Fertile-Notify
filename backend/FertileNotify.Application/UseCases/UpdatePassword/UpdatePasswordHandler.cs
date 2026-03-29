namespace FertileNotify.Application.UseCases.UpdatePassword
{
    public class UpdatePasswordHandler : IRequestHandler<UpdatePasswordCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UpdatePasswordHandler> _logger;

        public UpdatePasswordHandler(
            ISubscriberRepository subscriberRepository,
            IOtpService otpService,
            IEmailService emailService,
            ITokenService tokenService,
            ILogger<UpdatePasswordHandler> logger)
        {
            _subscriberRepository = subscriberRepository;
            _otpService = otpService;
            _emailService = emailService;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdatePasswordCommand command, CancellationToken cancellationToken)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            subscriber.UpdatePassword(command.CurrentPassword, Password.Create(command.NewPassword));
            await _subscriberRepository.SaveAsync(subscriber);
            return Unit.Value;
        }
    }
}
