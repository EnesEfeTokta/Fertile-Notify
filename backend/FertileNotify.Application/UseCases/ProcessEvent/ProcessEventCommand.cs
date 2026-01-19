namespace FertileNotify.Application.UseCases.ProcessEvent
{
    public class ProcessEventCommand
    {
        public Guid UserId { get; init; }
        public string EventType { get; init; } = default!;
        public string Payload { get; init; } = default!;
    }
}