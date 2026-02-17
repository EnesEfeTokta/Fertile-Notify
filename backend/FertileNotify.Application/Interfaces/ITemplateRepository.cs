using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.Interfaces
{
    public interface ITemplateRepository
    {
        Task<NotificationTemplate?> GetTemplateAsync(EventType eventType, NotificationChannel channel, Guid? subscriberId);
        Task<NotificationTemplate?> GetGlobalTemplateAsync(EventType eventType, NotificationChannel channel);
        Task<NotificationTemplate?> GetCustomTemplateAsync(EventType eventType, NotificationChannel channel, Guid subscriberId);
        Task<List<NotificationTemplate>> GetAllTemplatesAsync(Guid subscriberId);
        Task AddAsync(NotificationTemplate template);
        Task SaveAsync();
    }
}
