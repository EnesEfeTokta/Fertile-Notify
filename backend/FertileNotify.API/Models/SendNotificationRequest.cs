namespace FertileNotify.API.Models
{
    public class SendNotificationRequest
    {
        public string EventType { get; set; } = string.Empty;
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}