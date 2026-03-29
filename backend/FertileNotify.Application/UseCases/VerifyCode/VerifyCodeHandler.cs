namespace FertileNotify.Application.UseCases.VerifyCode
{
    public class VerifyCodeHandler : ICommandHandler<VerifyCodeCommand, LoginResponseDto>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IOtpService _otpService;
        private readonly ITokenService _tokenService;

        public VerifyCodeHandler(
            ISubscriberRepository subscriberRepository,
            ISubscriptionRepository subscriptionRepository,
            IOtpService otpService,
            ITokenService tokenService)
        {
            _subscriberRepository = subscriberRepository;
            _subscriptionRepository = subscriptionRepository;
            _otpService = otpService;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> Handle(VerifyCodeCommand command, CancellationToken cancellationToken)
        {
            var email = EmailAddress.Create(command.Email);

            var subscriber = await _subscriberRepository.GetByEmailAsync(email)
                ?? throw new NotFoundException("Subscriber not found.");

            var isValid = await _otpService.VerifyOtpAsync(subscriber.Id, command.Code);
            if (!isValid)
                throw new UnauthorizedException("Invalid or expired OTP code.");

            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(subscriber.Id)
                ?? throw new NotFoundException("Subscription not found.");

            var accessToken = _tokenService.GenerateToken(subscriber, subscription.Plan);
            var refreshToken = _tokenService.GenerateRefreshToken();

            subscriber.SetRefreshToken(refreshToken);
            await _subscriberRepository.SaveAsync(subscriber);

            return new LoginResponseDto(accessToken, refreshToken.Token.ToString());
        }
    }
}
