namespace FertileNotify.Application.UseCases.DeleteApiKey
{
    public class DeleteApiKeyHandler : IRequestHandler<DeleteApiKeyCommand, Unit>
    {
        private readonly IApiKeyRepository _apiKeyRepository;

        public DeleteApiKeyHandler(IApiKeyRepository apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        public async Task<Unit> Handle(DeleteApiKeyCommand command, CancellationToken cancellationToken)
        {
            var apiKey = await _apiKeyRepository.GetByIdAsync(command.ApiKeyId);

            if (apiKey == null || apiKey.SubscriberId != command.SubscriberId)
                throw new NotFoundException("API Key not found.");

            await _apiKeyRepository.DeleteByIdAsync(command.SubscriberId, command.ApiKeyId);

            return Unit.Value;
        }
    }
}
