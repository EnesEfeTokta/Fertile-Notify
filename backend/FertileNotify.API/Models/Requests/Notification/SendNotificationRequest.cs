namespace FertileNotify.API.Models.Requests
{
    public class SendNotificationRequest
    {
        public string EventType { get; set; } = string.Empty;
        public Dictionary<string, string> Parameters { get; set; } = new();

        public List<ChannelRecipientGroup> To { get; set; } = new();
    }

    public class ChannelRecipientGroup
    {
        public string Channel { get; set; } = string.Empty;
        public List<string> Recipients { get; set; } = new();
    }
}