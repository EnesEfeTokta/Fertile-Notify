namespace FertileNotify.Domain.ValueObjects
{
    public class NotificationContent
    {
        public string Subject { get; }
        public string Body { get; }

        public NotificationContent(string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(subject)) 
                throw new ArgumentException("Subject cannot be empty.");
            if (string.IsNullOrWhiteSpace(body)) 
                throw new ArgumentException("Body cannot be empty.");

            Subject = subject;
            Body = body;
        }

        public static NotificationContent Create(string subject, string body) 
            => new(subject, body);

        public NotificationContent Update(string subject, string body) 
            => new(subject, body);
    }
}
