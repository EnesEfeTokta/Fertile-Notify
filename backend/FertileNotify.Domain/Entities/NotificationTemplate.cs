using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class NotificationTemplate
    {
        public Guid Id { get; private set; }
        public Guid? SubscriberId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string? Description { get; private set; } = string.Empty;
        public EventType EventType { get; private set; } = default!;
        public NotificationChannel Channel { get; private set; } = default!;
        public NotificationContent Content { get; private set; } = default!;

        private NotificationTemplate() { }

        private NotificationTemplate(string name, string description, EventType eventType, NotificationChannel channel, NotificationContent content, Guid? subscriberId)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Name = name;
            Description = description;
            EventType = eventType;
            Channel = channel;
            Content = content;
        }

        public static NotificationTemplate CreateGlobal(string name, string description, EventType eventType, NotificationChannel channel, NotificationContent content)
            => new NotificationTemplate(name, description, eventType, channel, content, null);

        public static NotificationTemplate CreateCustom(Guid subscriberId, string name, string description, EventType eventType, NotificationChannel channel, NotificationContent content)
            => new NotificationTemplate(name, description, eventType, channel, content, subscriberId);

        public void Update(string name, string description, NotificationContent content)
        {
            Name = name;
            Description = description;
            Content = content;
        }
    }
}
