using FertileNotify.Domain.Events;

namespace FertileNotify.Domain.Entities
{
    public class NotificationTemplate
    {
        public Guid Id { get; private set; }
        public EventType EventType { get; private set; } = default!;
        public string SubjectTemplate { get; private set; } = string.Empty;
        public string BodyTemplate { get; private set; } = string.Empty;

        private NotificationTemplate(EventType eventType, string subject, string body) 
        {
            Id = Guid.NewGuid();
            EventType = eventType;
            SubjectTemplate = subject;
            BodyTemplate = body;
        }

        public static NotificationTemplate Create(EventType eventType, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject template cannot be null or empty.", nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Body template cannot be null or empty.", nameof(body));

            return new NotificationTemplate(eventType, subject, body);
        }
    }
}
