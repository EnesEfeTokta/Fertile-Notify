namespace FertileNotify.Application.UseCases.RevokeApiKey
{
    public class RevokeApiKeyHandler : IRequestHandler<RevokeApiKeyCommand, Unit>
    {
        private readonly IApiKeyRepository _apiKeyRepository;

        public RevokeApiKeyHandler(IApiKeyRepository apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        public async Task<Unit> Handle(RevokeApiKeyCommand command, CancellationToken cancellationToken)
        {
            var apiKeys = await _apiKeyRepository.GetBySubscriberIdAsync(command.SubscriberId);

            var keyToRevoke = apiKeys.FirstOrDefault(k => k.Id == command.ApiKeyId)
                ?? throw new NotFoundException("API Key not found.");

            keyToRevoke.Revoke();
            await _apiKeyRepository.SaveAsync(keyToRevoke);
            return Unit.Value;
        }
    }
}
