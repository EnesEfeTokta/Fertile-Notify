namespace FertileNotify.Application.Interfaces.Security
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(Guid subscriberId);
        Task<bool> VerifyOtpAsync(Guid subscriberId, string otp);
    }
}
