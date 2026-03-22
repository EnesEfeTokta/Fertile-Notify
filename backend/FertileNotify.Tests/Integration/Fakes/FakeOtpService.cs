using FertileNotify.Application.Interfaces;

namespace FertileNotify.Tests.Integration.Fakes
{
    public class FakeOtpService : IOtpService
    {
        public const string FixedOtp = "123456";

        public Task<string> GenerateOtpAsync(Guid subscriberId) 
            => Task.FromResult(FixedOtp);

        public Task<bool> VerifyOtpAsync(Guid subscriberId, string otp)
            => Task.FromResult(otp == FixedOtp);
    }
}
