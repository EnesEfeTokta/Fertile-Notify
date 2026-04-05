namespace FertileNotify.Application.Interfaces.Subscribers
{
    public interface IBlacklistRepository
    {
        Task<bool> IsBlacklistedAsync(Guid unwantedSubscriber, string address); 
        Task<List<ForbiddenRecipient>> GetForRecipientsAsync(Guid subscriberId, List<string> addresses);
        Task<List<ForbiddenRecipient>> GetAllBySubscriberAsync(Guid subscriberId);
        Task<ForbiddenRecipient?> GetByIdAsync(Guid id);
        Task AddOrUpdateAsync(ForbiddenRecipient recipient);
        Task DeleteAsync(Guid id);
        Task DeleteBySubscriberIdAsync(Guid subscriberId);
    }
}
