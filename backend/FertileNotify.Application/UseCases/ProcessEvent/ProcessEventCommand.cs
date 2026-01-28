using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Application.UseCases.ProcessEvent
{
    public class ProcessEventCommand
    {
        public Guid SubscriberId { get; init; }
        public NotificationChannel Channel { get; init; } = NotificationChannel.Email;
        public string Recipient { get; init; } = string.Empty;
        public EventType EventType { get; set; } = default!;
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
}