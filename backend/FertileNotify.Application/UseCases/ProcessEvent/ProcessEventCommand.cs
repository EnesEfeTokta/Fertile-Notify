using FertileNotify.Domain.Events;

namespace FertileNotify.Application.UseCases.ProcessEvent
{
    public class ProcessEventCommand
    {
        public Guid SubscriberId { get; init; }
        public string Recipient { get; init; } = string.Empty;
        public EventType EventType { get; set; } = default!;
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}