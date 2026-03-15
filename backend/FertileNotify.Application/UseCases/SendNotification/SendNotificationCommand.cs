namespace FertileNotify.Application.UseCases.SendNotification
{
    public class SendNotificationCommand
    {
        public Guid SubscriberId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public Dictionary<string, string> Parameters { get; set; } = new();
        public List<ChannelRecipientCommandGroup> To { get; set; } = new();
    }

    public class ChannelRecipientCommandGroup
    {
        public string Channel { get; set; } = string.Empty;
        public List<string> Recipients { get; set; } = new();
    }
}
