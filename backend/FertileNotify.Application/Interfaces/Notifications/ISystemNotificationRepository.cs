namespace FertileNotify.Application.Interfaces.Notifications
{
    public interface ISystemNotificationRepository
    {
        Task<List<SystemNotification>> GetAllByIsReadAsync(bool isRead = true);
        Task<SystemNotification?> GetByIdAsync(Guid notificationId);
        Task<List<SystemNotification>> GetBySubscriberIdAsync(Guid subscriberId, bool? isRead = null);
        Task AddAsync(SystemNotification notification);
        Task MarkAsReadAsync(Guid notificationId);
        Task DeleteAsync(Guid notificationId);
    }
}
