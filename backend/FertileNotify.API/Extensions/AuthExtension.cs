using FertileNotify.API.Authentication;
using FertileNotify.Infrastructure.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace FertileNotify.API.Extensions
{
    public static class AuthExtension
    {
        public static IServiceCollection AddAuthConfig(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITokenService, JwtTokenService>();
            services.AddScoped<ApiKeyService>();
            services.AddScoped<IOtpService, OtpService>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "JWT_OR_APIKEY";
                options.DefaultChallengeScheme = "JWT_OR_APIKEY";
            })
            .AddJwtBearer("Bearer", options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["JwtSettings:Issuer"] ?? "fertile-notify-api",
                    ValidAudience = configuration["JwtSettings:Audience"] ?? "fertile-notify-frontend",
                    IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]!))
                };
            })
            .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", null)
            .AddPolicyScheme("JWT_OR_APIKEY", "JWT_OR_APIKEY", options =>
            {
                options.ForwardDefaultSelector = context =>
                {
                    if (context.Request.Headers.ContainsKey("FN-Api-Key"))
                        return "ApiKey";
                    return "Bearer";
                };
            });

            return services;
        }
    }
}
