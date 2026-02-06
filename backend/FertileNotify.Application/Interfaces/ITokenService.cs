using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;

namespace FertileNotify.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Subscriber user, SubscriptionPlan plan);
    }
}
