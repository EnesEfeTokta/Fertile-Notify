using FertileNotify.Domain.Enums;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public DateTime CreateAt { get; private set; }

        public Notification(string title, string message)
        {
            Id = Guid.NewGuid();
            Title = title;
            Message = message;
            CreateAt = DateTime.UtcNow;
        }
    }
}