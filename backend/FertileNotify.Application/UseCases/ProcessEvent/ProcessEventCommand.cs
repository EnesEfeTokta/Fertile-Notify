namespace FertileNotify.Application.UseCases.ProcessEvent
{
    public class ProcessEventCommand
    {
        public Guid UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}