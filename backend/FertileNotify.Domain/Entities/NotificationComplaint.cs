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

        public NotificationContent Content { get; private set; } = default!;

        public DateTime CreatedAt { get; private set; }

        private NotificationComplaint() { }

        public NotificationComplaint(
            Guid subscriberId,
            EmailAddress reporterEmail,
            ComplaintType reason,
            string? description,
            NotificationContent content
        )
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            ReporterEmail = reporterEmail;
            Reason = reason;
            Description = description;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }

        public static NotificationComplaint Create(
            Guid subscriberId,
            EmailAddress reporterEmail,
            ComplaintType type,
            string? description,
            NotificationContent content
        )
        {
            return new NotificationComplaint(
                subscriberId,
                reporterEmail,
                type,
                description,
                content
            );
        }
    }
}
