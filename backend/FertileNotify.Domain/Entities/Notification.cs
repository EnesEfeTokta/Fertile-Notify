using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public NotificationContent Content { get; private set; }
        public DateTime CreateAt { get; private set; }

        private Notification()
        {
            Content = default!;
        }

        public Notification(NotificationContent content)
        {
            Id = Guid.NewGuid();
            Content = content;
            CreateAt = DateTime.UtcNow;
        }
    }
}