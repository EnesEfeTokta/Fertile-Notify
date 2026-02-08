using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;

namespace FertileNotify.Application.Interfaces
{
    public interface ITemplateRepository
    {
        Task<NotificationTemplate?> GetTemplateAsync(EventType eventType, Guid? subscriberId);
        Task<NotificationTemplate?> GetGlobalTemplateAsync(EventType eventType);
        Task<NotificationTemplate?> GetCustomTemplateAsync(EventType eventType, Guid subscriberId);
        Task AddAsync(NotificationTemplate template);
    }
}
