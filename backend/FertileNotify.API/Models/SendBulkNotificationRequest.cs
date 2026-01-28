namespace FertileNotify.API.Models
{
    public class SendBulkNotificationRequest
    {
        public string Channel { get; set; } = "Email";

        public List<string> Recipients { get; set; } = new();

        public string EventType { get; set; } = string.Empty;
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}
