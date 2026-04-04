namespace FertileNotify.Application.Interfaces.Security
{
    public interface ITokenService
    {
        string GenerateToken(Subscriber user, SubscriptionPlan plan);
        RefreshToken GenerateRefreshToken();
    }
}
