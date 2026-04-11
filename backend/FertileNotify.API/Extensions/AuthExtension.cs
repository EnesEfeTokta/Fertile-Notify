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
                var jwtSettings = configuration.GetSection("JwtSettings");
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
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
