namespace FertileNotify.API.Models
{
    public class SendNotificationRequest
    {
        public string Channel { get; set; } = "Email";
        public string Recipient { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}