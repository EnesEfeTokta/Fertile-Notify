namespace FertileNotify.API.Models.Requests
{
    public class AddWorkflowNotificationRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string Channels { get; set; } = default!;
        public string EventTrigger { get; set; } = string.Empty;
        public string CronExpression { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<ChannelRecipientGroup> To { get; set; } = new();
    }
}
