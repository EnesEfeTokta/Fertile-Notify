namespace FertileNotify.API.Models
{
    public class SendNotificationRequest
    {
        public Guid UserId { get; set; } = default(Guid);
        public string EventType { get; set; } = string.Empty;
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}