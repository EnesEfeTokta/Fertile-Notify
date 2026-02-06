using FertileNotify.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using System.Security.Cryptography;
using System.Security.Claims;

namespace FertileNotify.API.Authentication
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions {}

    public class ApiKeyAuthenticationHandler : AuthenticationHandler<ApiKeyAuthenticationOptions>
    {
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public ApiKeyAuthenticationHandler(
            IOptionsMonitor<ApiKeyAuthenticationOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            IApiKeyRepository apiKeyRepository,
            ISubscriptionRepository subscriptionRepository) 
            : base(options, logger, encoder)
        {
            _apiKeyRepository = apiKeyRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("FN-Api-Key", out var extractedApiKey))
                return AuthenticateResult.NoResult();

            var providedKey = extractedApiKey.ToString().Trim();

            if (string.IsNullOrWhiteSpace(providedKey))
                return AuthenticateResult.NoResult();

            try 
            {
                using var sha256 = SHA256.Create();
                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(providedKey));
                var hash = Convert.ToBase64String(hashBytes);

                var apiKey = await _apiKeyRepository.GetByKeyHashAsync(hash);

                if (apiKey == null)
                {
                    Logger.LogWarning("API Key Authentication failed. Key not found in database.");
                    return AuthenticateResult.Fail("Invalid API Key");
                }

                if (!apiKey.IsActive)
                {
                    Logger.LogWarning("API Key Authentication failed. Key is inactive. SubscriberId: {SubscriberId}", apiKey.SubscriberId);
                    return AuthenticateResult.Fail("API Key is inactive");
                }

                var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(apiKey.SubscriberId);
                var plan = subscription?.Plan.ToString() ?? "Free";

                var claims = new[] {
                    new Claim(ClaimTypes.NameIdentifier, apiKey.SubscriberId.ToString()),
                    new Claim("ApiKeyId", apiKey.Id.ToString()),
                    new Claim("Plan", plan)
                };

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                Logger.LogInformation("API Key Authentication successful. SubscriberId: {SubscriberId}", apiKey.SubscriberId);

                return AuthenticateResult.Success(ticket);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred during API Key authentication.");
                return AuthenticateResult.Fail("Authentication Error");
            }
        }
    }
}