using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;

namespace FertileNotify.Application.UseCases.CreateApiKey
{
    public class CreateApiKeyHandler
    {
        private readonly ApiKeyService _apiKeyService;

        public CreateApiKeyHandler(ApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        public async Task<string> HandleAsync(CreateApiKeyCommand command)
        {
            return await _apiKeyService.CreateApiKeyAsync(command.SubscriberId, command.Name);
        }
    }
}
