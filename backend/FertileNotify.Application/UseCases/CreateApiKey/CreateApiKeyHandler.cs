namespace FertileNotify.Application.UseCases.CreateApiKey
{
    public class CreateApiKeyHandler: IRequestHandler<CreateApiKeyCommand, string>
    {
        private readonly ApiKeyService _apiKeyService;

        public CreateApiKeyHandler(ApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        public async Task<string> Handle(CreateApiKeyCommand command, CancellationToken cancellationToken)
            => await _apiKeyService.CreateApiKeyAsync(command.SubscriberId, command.Name);
    }
}
