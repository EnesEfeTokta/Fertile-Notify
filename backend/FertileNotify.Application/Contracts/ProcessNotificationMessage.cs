namespace FertileNotify.Application.Contracts
{
    public class ProcessNotificationMessage
    {
        public Guid SubscriberId { get; init; }
        public string Recipient { get; init; } = string.Empty;
        public string EventType { get; init; } = string.Empty;
        public string Channel { get; init; } = string.Empty;
        public Dictionary<string, string> Parameters { get; init; } = new();
    }
}
