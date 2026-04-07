using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class ForbiddenRecipient
    {
        public Guid Id { get; private set; }
        public Guid UnwantedSubscriber { get; private set; }
        public string RecipientAddress { get; private set; } = string.Empty;
        public List<NotificationChannel> UnwantedChannels { get; private set; } = new();
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }

        private ForbiddenRecipient() { }

        public ForbiddenRecipient(Guid unwantedSubscriber, string recipientAddress, List<NotificationChannel> unwantedChannels)
        {
            Id = Guid.NewGuid();
            UnwantedSubscriber = unwantedSubscriber;
            RecipientAddress = recipientAddress;
            UnwantedChannels = unwantedChannels;
            CreatedAt = DateTime.UtcNow;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void UpdateChannels(List<NotificationChannel> channels)
        {
            UnwantedChannels = channels ?? new List<NotificationChannel>();
        }
    }
}
