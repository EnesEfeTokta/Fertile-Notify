namespace FertileNotify.Application.Interfaces.Observability
{
    public interface IStatsRepository
    {
        Task IncrementAsync(
            Guid subscriberId, 
            NotificationChannel channel, 
            EventType eventType, 
            bool isSuccess);

        Task<List<SubscriberDailyStats>> GetStatsAsync(
            Guid subscriberId, 
            DateTime startDate, 
            DateTime endDate);

        Task DeleteBySubscriberIdAsync(Guid subscriberId);
    }
}
