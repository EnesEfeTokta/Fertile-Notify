using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(Subscriber user);
    }
}
