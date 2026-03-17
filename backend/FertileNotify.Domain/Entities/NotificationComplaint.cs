using FertileNotify.Domain.Enums;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class NotificationComplaint
    {
        public Guid Id { get; private set; }
        public Guid SubscriberId { get; private set; }

        public EmailAddress ReporterEmail { get; private set; } = default!;

        public ComplaintType Reason { get; private set; }
        public string? Description { get; private set; }

        public string NotificationSubject { get; private set; } = string.Empty;
        public string NotificationBody { get; private set; } = string.Empty;

        public DateTime CreatedAt { get; private set; }

        private NotificationComplaint() { }

        public NotificationComplaint(
            Guid subscriberId,
            EmailAddress reporterEmail,
            ComplaintType reason,
            string? description,
            string notificationSubject,
            string notificationBody
        )
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            ReporterEmail = reporterEmail;
            Reason = reason;
            Description = description;
            NotificationSubject = notificationSubject;
            NotificationBody = notificationBody;
            CreatedAt = DateTime.UtcNow;
        }

        public static NotificationComplaint Create(
            Guid subscriberId,
            EmailAddress reporterEmail,
            ComplaintType type,
            string? description,
            string notificationSubject,
            string notificationBody
        )
        {
            return new NotificationComplaint(
                subscriberId,
                reporterEmail,
                type,
                description,
                notificationSubject,
                notificationBody
            );
        }
    }
}
