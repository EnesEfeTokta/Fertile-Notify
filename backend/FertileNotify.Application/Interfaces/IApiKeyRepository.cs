using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface IApiKeyRepository
    {
        Task SaveAsync(ApiKey apiKey);
        Task<ApiKey?> GetByKeyHashAsync(string keyHash);
        Task<List<ApiKey>> GetBySubscriberIdAsync(Guid subscriberId);
    }
}
