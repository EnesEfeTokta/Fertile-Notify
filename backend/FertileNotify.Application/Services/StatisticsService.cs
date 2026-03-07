using FertileNotify.Application.DTOs;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Exceptions;

namespace FertileNotify.Application.Services
{
    public class StatisticsService : IStatisticsService
    {
        private readonly INotificationLogRepository _logRepository;
        private readonly IStatsRepository _statsRepository;

        public StatisticsService(INotificationLogRepository logRepository, IStatsRepository statsRepository)
        {
            _logRepository = logRepository;
            _statsRepository = statsRepository;
        }

        public async Task<StatisticsDto> GetSubscriberStatsAsync(Guid subscriberId, string period, SubscriptionPlan plan)
        {
            ValidatePeriodAccess(period, plan);
            DateTime startDate = CalculateStartDate(period);
            var statsData = await _statsRepository.GetStatsAsync(subscriberId, startDate, DateTime.UtcNow);

            return new StatisticsDto
            {
                TotalUsage = statsData.Sum(s => s.SuccessCount + s.FailedCount),
                SuccessCount = statsData.Sum(s => s.SuccessCount),
                FailedCount = statsData.Sum(s => s.FailedCount),

                StatsByChannel = statsData
                    .GroupBy(s => s.Channel)
                    .ToDictionary(
                        g => g.Key.Name,
                        g => g.Sum(s => s.SuccessCount + s.FailedCount)
                    ),

                StatsByEventType = statsData
                    .GroupBy(s => s.EventType)
                    .ToDictionary(
                        g => g.Key.Name,
                        g => g.Sum(s => s.SuccessCount + s.FailedCount)
                    )
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
