using FertileNotify.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace FertileNotify.Tests.Integration.Fakes;

public class FakeOtpService : IOtpService
{
    public const string FixedOtp = "123456";

    public Task<string> GenerateOtpAsync(Guid subscriberId)
    {
        // Always return a predictable OTP code for predictability in tests
        return Task.FromResult(FixedOtp);
    }

    public Task<bool> VerifyOtpAsync(Guid subscriberId, string otp)
    {
        // Accept only the fixed OTP
        return Task.FromResult(otp == FixedOtp);
    }
}
