using FertileNotify.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

namespace FertileNotify.Infrastructure.Authentication
{
    public class OtpService : IOtpService
    {
        private readonly IDistributedCache _cache;

        public OtpService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<string> GenerateOtpAsync(Guid subscriberId)
        {
            var otp = new Random().Next(100000, 999999).ToString();
            var key = $"otp_{subscriberId}";

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync(key, otp, options);

            Console.WriteLine($"Generated OTP for subscriber {subscriberId}: {otp}"); // For development purposes only
            return otp;
        }

        public async Task<bool> VerifyOtpAsync(Guid subscriberId, string otp)
        {
            var key = $"otp_{subscriberId}";
            var cachedOtp = await _cache.GetStringAsync(key);

            if (!string.IsNullOrEmpty(cachedOtp) && cachedOtp == otp)
            {
                await _cache.RemoveAsync(key);
                return true;
            }
            return false;
        }
    }
}
