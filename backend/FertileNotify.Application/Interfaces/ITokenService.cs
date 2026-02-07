using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Subscriber user, SubscriptionPlan plan);
        RefreshToken GenerateRefreshToken();
    }
}
