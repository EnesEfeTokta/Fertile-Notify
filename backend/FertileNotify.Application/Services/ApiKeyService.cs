using System.Security.Cryptography;
using System.Text;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Application.Services
{
    public class ApiKeyService
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ILogger<ApiKeyService> _logger;

        public ApiKeyService(IApiKeyRepository apiKeyRepository, ILogger<ApiKeyService> logger)
        {
            _apiKeyRepository = apiKeyRepository;
            _logger = logger;
        }

        public async Task<string> CreateApiKeyAsync(Guid subscriberId, string name)
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            var key = "fn_" + Convert.ToBase64String(randomBytes).Replace("+", "").Replace("/", "").Substring(0, 30);

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            var hash = Convert.ToBase64String(hashBytes);

            var apiKey = new ApiKey(subscriberId, hash, key.Substring(0, 7), name);
            await _apiKeyRepository.SaveAsync(apiKey);

            _logger.LogInformation("New API Key created for Subscriber: {SubscriberId}. Name: {Name}", subscriberId, name);

            return key;
        }
    }
}
