namespace FertileNotify.Application.UseCases.Login
{
    public class LoginHandler: IRequestHandler<LoginCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;

        public LoginHandler(ISubscriberRepository subscriberRepository, IOtpService otpService, IEmailService emailService)
        { 
            _subscriberRepository = subscriberRepository;
            _otpService = otpService;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(LoginCommand command, CancellationToken ct)
        {
            var subscriber = await _subscriberRepository.GetByEmailAsync(EmailAddress.Create(command.Email))
                ?? throw new NotFoundException("Subscriber not found");

            if (!subscriber.Password.Verify(command.Password))
                throw new UnauthorizedException("Invalid credentials");

            var otpCode = await _otpService.GenerateOtpAsync(subscriber.Id);
            await _emailService.SendEmailAsync(subscriber.Email, "OTP", $"Code: {otpCode}");
            return Unit.Value;
        }
    }
}
