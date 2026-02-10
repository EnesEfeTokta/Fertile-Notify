using FertileNotify.Domain.Events;

namespace FertileNotify.Domain.Entities
{
    public class NotificationTemplate
    {
        public Guid Id { get; private set; }
        public Guid? SubscriberId { get; private set; }
        public EventType EventType { get; private set; } = default!;
        public string SubjectTemplate { get; private set; } = string.Empty;
        public string BodyTemplate { get; private set; } = string.Empty;

        private NotificationTemplate() { }

        private NotificationTemplate(EventType eventType, string subject, string body, Guid? subscriberId)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            EventType = eventType;
            SubjectTemplate = subject;
            BodyTemplate = body;
        }

        public static NotificationTemplate CreateGlobal(EventType eventType, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject template cannot be null or empty.", nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body template cannot be null or empty.", nameof(body));

            return new NotificationTemplate(eventType, subject, body, null);
        }

        public static NotificationTemplate CreateCustom(Guid subscriberId, EventType eventType, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject template cannot be null or empty.", nameof(subject));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body template cannot be null or empty.", nameof(body));

            return new NotificationTemplate(eventType, subject, body, subscriberId);
        }

        public void Update(string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject template cannot be null or empty.", nameof(subject));

            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body template cannot be null or empty.", nameof(body));

            SubjectTemplate = subject;
            BodyTemplate = body;
        }
    }
}
