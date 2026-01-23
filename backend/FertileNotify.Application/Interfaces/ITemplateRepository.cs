using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;

namespace FertileNotify.Application.Interfaces
{
    public interface ITemplateRepository
    {
        Task<NotificationTemplate?> GetByEventTypeAsync(EventType eventType);
        Task AddAsync(NotificationTemplate template);
    }
}
