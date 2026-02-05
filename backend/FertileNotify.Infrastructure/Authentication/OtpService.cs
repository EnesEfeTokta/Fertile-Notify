using FertileNotify.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace FertileNotify.Infrastructure.Authentication
{
    public class OtpService : IOtpService
    {
        private readonly IMemoryCache _cache;

        public OtpService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<string> GenerateOtpAsync(Guid subscriberId)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var key = $"otp_{subscriberId}";

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

            _cache.Set(key, otp, cacheEntryOptions);
            Console.WriteLine($"Generated OTP for subscriber {subscriberId}: {otp}"); // For development purposes only
            return Task.FromResult(otp);
        }

        public Task<bool> VerifyOtpAsync(Guid subscriberId, string otp)
        {
            var key = $"otp_{subscriberId}";
            if (_cache.TryGetValue(key, out string? cachedOtp))
            {
                if (cachedOtp == otp)
                {
                    _cache.Remove(key);
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }
    }
}
