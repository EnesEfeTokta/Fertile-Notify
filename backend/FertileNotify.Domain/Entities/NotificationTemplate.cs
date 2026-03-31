using System;
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
        public string Subject { get; private set; } = string.Empty;
        public string Body { get; private set; } = string.Empty;

        private NotificationTemplate() { }

        private NotificationTemplate(string name, string description, EventType eventType, NotificationChannel channel, string subject, string body, Guid? subscriberId)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Name = name;
            Description = description;
            EventType = eventType;
            Channel = channel;
            Subject = subject;
            Body = body;
        }

        public static NotificationTemplate CreateGlobal(string name, string description, EventType eventType, NotificationChannel channel, string subject, string body)
            => new NotificationTemplate(name, description, eventType, channel, subject, body, null);

        public static NotificationTemplate CreateCustom(Guid subscriberId, string name, string description, EventType eventType, NotificationChannel channel, string subject, string body)
            => new NotificationTemplate(name, description, eventType, channel, subject, body, subscriberId);

        public void Update(string name, string description, string subject, string body)
        {
            Name = name;
            Description = description;
            Subject = subject;
            Body = body;
        }
    }
}
