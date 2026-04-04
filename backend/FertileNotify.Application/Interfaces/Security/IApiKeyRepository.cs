namespace FertileNotify.Application.Interfaces.Security
{
    public interface IApiKeyRepository
    {
        Task SaveAsync(ApiKey apiKey);
        Task<ApiKey?> GetByKeyHashAsync(string keyHash);
        Task<List<ApiKey>> GetBySubscriberIdAsync(Guid subscriberId);
    }
}
