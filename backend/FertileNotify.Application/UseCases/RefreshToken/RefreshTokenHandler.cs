namespace FertileNotify.Application.UseCases.RefreshToken
{
    public class RefreshTokenHandler: IRequestHandler<RefreshTokenCommand, LoginResponseDto>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ITokenService _tokenService;

        public RefreshTokenHandler(
            ISubscriberRepository subscriberRepository,
            ISubscriptionRepository subscriptionRepository,
            ITokenService tokenService)
        {
            _subscriberRepository = subscriberRepository;
            _subscriptionRepository = subscriptionRepository;
            _tokenService = tokenService;
        }

        public async Task<LoginResponseDto> Handle(RefreshTokenCommand command, CancellationToken ct)
        {
            var subscriber = await _subscriberRepository.GetByRefreshTokenAsync(command.RefreshToken)
                                ?? throw new UnauthorizedException("Invalid or expired refresh token");

            if (!subscriber.RefreshToken!.IsActive())
                throw new UnauthorizedException("Refresh token has expired");

            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(subscriber.Id)
                    ?? throw new NotFoundException("Subscription not found");

            var newAccessToken = _tokenService.GenerateToken(subscriber, subscription.Plan);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            subscriber.SetRefreshToken(newRefreshToken);
            await _subscriberRepository.SaveAsync(subscriber);

            return new LoginResponseDto(newAccessToken, newRefreshToken.Token);
        }
    }
}
