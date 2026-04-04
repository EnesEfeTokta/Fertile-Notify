namespace FertileNotify.Application.Interfaces.Notifications
{
    public interface INotificationComplaintRepository
    {
        Task SaveAsync(NotificationComplaint complaint);
        Task<NotificationComplaint?> GetByIdAsync(Guid id);
        Task<List<NotificationComplaint>> GetComplaintsBySubscriberIdAsync(Guid subscriberId);
    }
}
