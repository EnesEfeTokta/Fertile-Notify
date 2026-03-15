using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Exceptions;

namespace FertileNotify.Application.UseCases.RevokeApiKey
{
    public class RevokeApiKeyHandler
    {
        private readonly IApiKeyRepository _apiKeyRepository;

        public RevokeApiKeyHandler(IApiKeyRepository apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        public async Task HandleAsync(RevokeApiKeyCommand command)
        {
            var apiKeys = await _apiKeyRepository.GetBySubscriberIdAsync(command.SubscriberId);

            var keyToRevoke = apiKeys.FirstOrDefault(k => k.Id == command.ApiKeyId)
                ?? throw new NotFoundException("API Key not found.");

            keyToRevoke.Revoke();
            await _apiKeyRepository.SaveAsync(keyToRevoke);
        }
    }
}
