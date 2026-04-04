namespace FertileNotify.Application.Interfaces.Notifications
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
