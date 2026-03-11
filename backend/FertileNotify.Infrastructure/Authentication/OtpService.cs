using FertileNotify.Application.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.Authentication
{
    public class OtpService : IOtpService
    {
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private readonly ILogger<OtpService> _logger;

        public OtpService(IDistributedCache cache, IConfiguration configuration, ILogger<OtpService> logger)
        {
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GenerateOtpAsync(Guid subscriberId)
        {
            int length = _configuration.GetValue<int>("OTPSettings:Length", 6);
            int expiryInMinutes = _configuration.GetValue<int>("OTPSettings:ExpiryInMinutes", 5);

            var otp = new Random().Next((int)Math.Pow(10, length - 1), (int)Math.Pow(10, length)).ToString();
            var key = $"otp_{subscriberId}";

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(expiryInMinutes)
            };

            await _cache.SetStringAsync(key, otp, options);
            _logger.LogInformation("OTP generated for subscriber {SubscriberId}", subscriberId);
            return otp;
        }

        public async Task<bool> VerifyOtpAsync(Guid subscriberId, string otp)
        {
            var key = $"otp_{subscriberId}";
            var cachedOtp = await _cache.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cachedOtp) && cachedOtp == otp)
            {
                await _cache.RemoveAsync(key);
                _logger.LogInformation("OTP verified for subscriber {SubscriberId}", subscriberId);
                return true;
            }
            
            _logger.LogInformation("Invalid OTP for subscriber {SubscriberId}", subscriberId);
            return false;
        }
    }
}
