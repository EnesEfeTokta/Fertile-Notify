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
        public string SubjectTemplate { get; private set; } = string.Empty;
        public string BodyTemplate { get; private set; } = string.Empty;

        private NotificationTemplate() { }

        private NotificationTemplate(string name, string description, EventType eventType, NotificationChannel channel, string subject, string body, Guid? subscriberId)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Name = name;
            Description = description;
            EventType = eventType;
            Channel = channel;
            SubjectTemplate = subject;
            BodyTemplate = body;
        }

        public static NotificationTemplate CreateGlobal(string name, string description, EventType eventType, NotificationChannel channel, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject template cannot be null or empty.", nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body template cannot be null or empty.", nameof(body));

            return new NotificationTemplate(name, description, eventType, channel, subject, body, null);
        }

        public static NotificationTemplate CreateCustom(Guid subscriberId, string name, string description, EventType eventType, NotificationChannel channel, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject template cannot be null or empty.", nameof(subject));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body template cannot be null or empty.", nameof(body));

            return new NotificationTemplate(name, description, eventType, channel, subject, body, subscriberId);
        }

        public void Update(string name, string description, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("description", nameof(description));

            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject template cannot be null or empty.", nameof(subject));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body template cannot be null or empty.", nameof(body));

            Name = name;
            Description = description;

            SubjectTemplate = subject;
            BodyTemplate = body;
        }
    }
}
