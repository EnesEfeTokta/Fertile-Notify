using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Services
{
    public class ApiKeyService
    {
        private readonly IApiKeyRepository _apiKeyRepository;

        public ApiKeyService(IApiKeyRepository apiKeyRepository)
        {
            _apiKeyRepository = apiKeyRepository;
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

            return key;
        }
    }
}
