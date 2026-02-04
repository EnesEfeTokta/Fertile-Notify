using FertileNotify.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;

namespace FertileNotify.API.Authentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions { }

    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IApiKeyRepository _apiKeyRepository;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IApiKeyRepository apiKeyRepository)
            : base(options, logger, encoder)
        {
            _apiKeyRepository = apiKeyRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("FN-Api-Key", out var extractedApiKey))
                AuthenticateResult.NoResult();

            var providedKey = extractedApiKey.ToString();

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(providedKey));
            var hash = Convert.ToBase64String(hashBytes);

            var apiKey = await _apiKeyRepository.GetByKeyHashAsync(hash);
            if (apiKey == null)
                return AuthenticateResult.Fail("Invalid API Key");

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, apiKey.SubscriberId.ToString()),
                new Claim("ApiKeyId", apiKey.Id.ToString())
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }
    }
}
