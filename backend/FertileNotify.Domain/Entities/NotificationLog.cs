using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class NotificationLog
    {
        public Guid Id { get; private set; }
        public Guid SubscriberId { get; private set; }
        public string Recipient { get; private set; } = string.Empty;
        public NotificationChannel Channel { get; private set; } = default!;
        public EventType EventType { get; private set; } = default!;
        public string Subject { get; private set; } = string.Empty;
        public string Body { get; private set; } = string.Empty;
        public DeliveryStatus Status { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private NotificationLog() { }

        public NotificationLog(Guid subscriberId, string recipient, NotificationChannel channel, EventType eventType, string subject, string body)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Recipient = recipient;
            Channel = channel;
            EventType = eventType;
            Subject = subject;
            Body = body;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetResult(bool isSuccess, string? error = null)
        {
            Status = isSuccess ? DeliveryStatus.Success : DeliveryStatus.Failed;
            ErrorMessage = error;
        }
    }
}