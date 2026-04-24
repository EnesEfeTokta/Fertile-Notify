namespace FertileNotify.Application.UseCases.UpdateApiKeyStatus
{
    public class UpdateApiKeyStatusHandler : IRequestHandler<UpdateApiKeyStatusCommand, Unit>
    {
        private readonly IApiKeyRepository _apiKeyRepository;

        public UpdateApiKeyStatusHandler(IApiKeyRepository apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        public async Task<Unit> Handle(UpdateApiKeyStatusCommand command, CancellationToken cancellationToken)
        {
            var apiKey = await _apiKeyRepository.GetByIdAsync(command.ApiKeyId);

            if (apiKey == null || apiKey.SubscriberId != command.SubscriberId)
                throw new NotFoundException("API Key not found.");

            apiKey.SetActive(command.IsActive);
            await _apiKeyRepository.SaveAsync(apiKey);

            return Unit.Value;
        }
    }
}
