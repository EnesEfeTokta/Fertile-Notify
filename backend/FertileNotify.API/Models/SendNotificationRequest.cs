namespace FertileNotify.API.Models
{
    public class SendNotificationRequest
    {
        public Guid UserId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Payload { get; set; } = string.Empty;
    }
}
