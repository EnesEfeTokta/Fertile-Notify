namespace FertileNotify.Application.Interfaces.Notifications
{
    public interface INotificationLogRepository
    {
        Task AddAsync(NotificationLog log);
        Task AddRangeAsync(IEnumerable<NotificationLog> logs);
        Task<List<NotificationLog>> GetLatestBySubscriberIdAsync(Guid subscriberId, int count);
        Task<List<NotificationLog>> GetLogsForStatsAsync(Guid subscriberId, DateTime startDate);
        
        // GDPR Methods
        Task<List<NotificationLog>> GetLogsForAnonymizationAsync(DateTime cutOffDate);
        Task UpdateRangeAsync(IEnumerable<NotificationLog> logs);
        Task DeleteLogsOlderThanAsync(DateTime cutOffDate);
    }
}