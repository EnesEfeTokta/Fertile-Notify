namespace FertileNotify.Application.Interfaces.Security
{
    public interface IApiKeyRepository
    {
        Task SaveAsync(ApiKey apiKey);
        Task<ApiKey?> GetByKeyHashAsync(string keyHash);
        Task<ApiKey?> GetByIdAsync(Guid apiKeyId);
        Task<List<ApiKey>> GetBySubscriberIdAsync(Guid subscriberId);
        Task DeleteByIdAsync(Guid subscriberId, Guid apiKeyId);
        Task DeleteBySubscriberIdAsync(Guid subscriberId);
    }
}
