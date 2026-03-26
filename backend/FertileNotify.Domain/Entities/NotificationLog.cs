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
        public NotificationContent Content { get; private set; } = default!;
        public DeliveryStatus Status { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime CreatedAt { get; private set; }

        private NotificationLog() { }

        public NotificationLog(Guid subscriberId, string recipient, NotificationChannel channel, EventType eventType, NotificationContent content)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Recipient = recipient;
            Channel = channel;
            EventType = eventType;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }

        public void SetResult(bool isSuccess, string? error = null, bool isRejected = false)
        {
            if (isRejected)
                Status = DeliveryStatus.Rejected;
            else
                Status = isSuccess ? DeliveryStatus.Success : DeliveryStatus.Failed;

            ErrorMessage = error;
        }

        public void AnonymizeContent()
        {
            Content = new NotificationContent(MaskVariables(Content.Subject), MaskVariables(Content.Body));
        }

        private string MaskVariables(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            
            // Regex to find {{VariableName}} or {{ VariableName }}
            return System.Text.RegularExpressions.Regex.Replace(input, @"\{\{.*?\}\}", "***");
        }
    }
}