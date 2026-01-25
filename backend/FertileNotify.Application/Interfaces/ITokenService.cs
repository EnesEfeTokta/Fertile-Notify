using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
