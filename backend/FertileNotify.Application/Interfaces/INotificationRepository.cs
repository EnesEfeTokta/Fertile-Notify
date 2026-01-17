using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface INotificationRepository
    {
        Task SendAsync(Notification notification);
    }
}