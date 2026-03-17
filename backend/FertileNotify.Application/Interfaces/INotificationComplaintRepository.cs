using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface INotificationComplaintRepository
    {
        Task SaveAsync(NotificationComplaint complaint);
        Task<NotificationComplaint?> GetByIdAsync(Guid id);
        Task<List<NotificationComplaint>> GetComplaintsBySubscriberIdAsync(Guid subscriberId);
    }
}
