using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Interfaces
{
    public interface IStatsRepository
    {
        Task IncrementAsync(Guid subscriberId, NotificationChannel channel, EventType eventType, bool isSuccess);
        Task<List<SubscriberDailyStats>> GetStatsAsync(Guid subscriberId, DateTime startDate, DateTime endDate);
    }
}
