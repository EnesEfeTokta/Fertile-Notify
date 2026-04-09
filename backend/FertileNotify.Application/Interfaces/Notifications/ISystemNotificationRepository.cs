namespace FertileNotify.Application.Interfaces.Notifications
{
    public interface ISystemNotificationRepository
    {
        Task<List<SystemNotification>> GetAllByIsReadAsync(bool isRead = true);
        Task<List<SystemNotification>> GetBySubscriberIdAsync(Guid subscriberId);
        Task AddAsync(SystemNotification notification);
        Task MarkAsReadAsync(Guid notificationId);
        Task DeleteAsync(Guid notificationId);
    }
}
