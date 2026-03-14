namespace FertileNotify.Application.Interfaces
{
    public interface ISecurityService
    {
        string GenerateUnsubscribeToken(string email, Guid subscriberId);
        bool VerifyUnsubscribeToken(string email, Guid subscriberId, string token);
    }
}