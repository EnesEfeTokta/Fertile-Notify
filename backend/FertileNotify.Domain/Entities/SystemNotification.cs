namespace FertileNotify.Domain.Entities
{
    public class SystemNotification
    {
        public Guid Id { get; private set; }
        public Guid SubscriberId { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }

        public SystemNotification(Guid subscriberId, string title, string message)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Title = title;
            Message = message;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead() => IsRead = true;
    }
}
