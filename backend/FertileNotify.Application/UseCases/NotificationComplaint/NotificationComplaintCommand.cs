namespace FertileNotify.Application.UseCases.NotificationComplaint
{
    public class NotificationComplaintCommand : ICommand
    {
        public Guid SubscriberId { get; set; }
        public string ReporterEmail { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public string? Description { get; set; }
        public string Subject { get; set; } = null!;
        public string Body { get; set; } = null!;
    }
}
