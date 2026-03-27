namespace FertileNotify.Application.Contracts
{
    public class ProcessNotificationMessage
    {
        public Guid SubscriberId { get; init; }
        public Guid? WorkflowId { get; init; }
        public string Recipient { get; init; } = string.Empty;
        public string EventType { get; init; } = string.Empty;
        public string Channel { get; init; } = string.Empty;
        public Dictionary<string, string> Parameters { get; init; } = new();
        public string? DirectSubject { get; init; }
        public string? DirectBody { get; init; }
    }
}
