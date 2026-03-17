namespace FertileNotify.Application.UseCases.NotificationComplaint
{
    public class NotificationComplaintCommand
    {
        public Guid SubscriberId { get; set; }
        public string ReporterEmail { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public string? Description { get; set; }
        public string NotificationSubject { get; set; } = null!;
        public string NotificationBody { get; set; } = null!;
    }
}
