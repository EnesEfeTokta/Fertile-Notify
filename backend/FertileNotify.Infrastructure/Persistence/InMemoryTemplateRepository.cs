using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;

namespace FertileNotify.Infrastructure.Persistence
{
    public class InMemoryTemplateRepository : ITemplateRepository
    {
        private readonly List<NotificationTemplate> _templates = new();

        public InMemoryTemplateRepository() 
        {
            _templates.Add(NotificationTemplate.Create(
                EventType.UserRegistered, 
                "Welcome, {Name}!", 
                "Hello {Name}, thank you for registering at our site. Your email: {Email}"));

            _templates.Add(NotificationTemplate.Create(
                EventType.OrderCreated,
                "Order Created",
                "Hello {Name}, your order has been created."));
        }

        public Task AddAsync(NotificationTemplate template)
        {
            _templates.Add(template);
            return Task.CompletedTask;
        }

        public Task<NotificationTemplate?> GetByEventTypeAsync(EventType eventType)
        {
            return Task.FromResult(_templates.FirstOrDefault(t => t.EventType.Equals(eventType)));
        }
    }
}
