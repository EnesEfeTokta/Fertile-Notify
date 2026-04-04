namespace FertileNotify.Application.Interfaces.Observability
{
    public interface IStatisticsService
    {
        Task<StatisticsDto> GetSubscriberStatsAsync(
            Guid subscriberId, 
            string period, 
            SubscriptionPlan plan);
    }
}
