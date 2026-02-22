using FertileNotify.Application.DTOs;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Exceptions;

namespace FertileNotify.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly INotificationLogRepository _logRepository;

        public StatisticsService(INotificationLogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        public async Task<StatisticsDto> GetSubscriberStatsAsync(Guid subscriberId, string period, SubscriptionPlan plan)
        {
            ValidatePeriodAccess(period, plan);

            DateTime startDate = CalculateStartDate(period);

            var logs = await _logRepository.GetLogsForStatsAsync(subscriberId, startDate);

            return new StatisticsDto
            {
                TotalUsage = logs.Count,
                SuccessCount = logs.Count(l => l.Status == DeliveryStatus.Success),
                FailedCount = logs.Count(l => l.Status == DeliveryStatus.Failed),
                StatsByChannel = logs.GroupBy(l => l.Channel).ToDictionary(g => g.Key.Name, g => g.Count()),
                StatsByEventType = logs.GroupBy(l => l.EventType).ToDictionary(g => g.Key.Name, g => g.Count())
            };
        }

        private void ValidatePeriodAccess(string period, SubscriptionPlan plan)
        {
            if (plan == SubscriptionPlan.Free && (period != "daily" && period != "weekly"))
                throw new BusinessRuleException("Free plan only supports daily and weekly statistics.");

            if (plan == SubscriptionPlan.Pro && (period == "6months" || period == "1year"))
                throw new BusinessRuleException("Pro plan supports up to 3 months of statistics.");
        }

        private DateTime CalculateStartDate(string period) => period switch
        {
            "daily" => DateTime.UtcNow.AddDays(-1),
            "weekly" => DateTime.UtcNow.AddDays(-7),
            "1month" => DateTime.UtcNow.AddMonths(-1),
            "3months" => DateTime.UtcNow.AddMonths(-3),
            "6months" => DateTime.UtcNow.AddMonths(-6),
            "1year" => DateTime.UtcNow.AddYears(-1),
            _ => DateTime.UtcNow.AddDays(-1)
        };
    }
}
