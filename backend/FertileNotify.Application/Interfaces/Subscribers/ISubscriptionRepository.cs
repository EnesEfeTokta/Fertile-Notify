namespace FertileNotify.Application.Interfaces.Subscribers
{
    public interface ISubscriptionRepository
    {
        Task SaveAsync(Guid subscriberId, Subscription subscription);
        Task<Subscription?> GetBySubscriberIdAsync(Guid subscriberId);
    }
}
