namespace FertileNotify.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public string Subject { get; private set; }
        public string Body { get; private set; }
        public DateTime CreateAt { get; private set; }

        public Notification(string subject, string body)
        {
            Id = Guid.NewGuid();
            Subject = subject;
            Body = body;
            CreateAt = DateTime.UtcNow;
        }
    }
}