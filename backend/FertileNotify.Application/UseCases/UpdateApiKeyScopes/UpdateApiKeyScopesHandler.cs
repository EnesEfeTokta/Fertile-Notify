namespace FertileNotify.Application.UseCases.UpdateApiKeyScopes
{
    public class UpdateApiKeyScopesHandler : IRequestHandler<UpdateApiKeyScopesCommand, Unit>
    {
        private readonly IApiKeyRepository _apiKeyRepository;

        public UpdateApiKeyScopesHandler(IApiKeyRepository apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        public async Task<Unit> Handle(UpdateApiKeyScopesCommand command, CancellationToken cancellationToken)
        {
            var apiKey = await _apiKeyRepository.GetByIdAsync(command.ApiKeyId);

            if (apiKey == null || apiKey.SubscriberId != command.SubscriberId)
                throw new NotFoundException("API Key not found.");

            apiKey.SetScopes(command.Scopes);
            await _apiKeyRepository.SaveAsync(apiKey);

            return Unit.Value;
        }
    }
}
