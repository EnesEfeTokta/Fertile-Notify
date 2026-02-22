using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface INotificationLogRepository
    {
        Task AddAsync(NotificationLog log);
        Task<List<NotificationLog>> GetLatestBySubscriberIdAsync(Guid subscriberId, int count);
        Task<List<NotificationLog>> GetLogsForStatsAsync(Guid subscriberId, DateTime startDate);
    }
}