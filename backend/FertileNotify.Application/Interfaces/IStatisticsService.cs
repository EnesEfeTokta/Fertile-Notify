using FertileNotify.Application.DTOs;
using FertileNotify.Domain.Enums;

namespace FertileNotify.Application.Interfaces
{
    public interface IStatisticsService
    {
        Task<StatisticsDto> GetSubscriberStatsAsync(Guid subscriberId, string period, SubscriptionPlan plan);
    }
}
