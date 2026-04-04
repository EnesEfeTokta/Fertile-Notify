namespace FertileNotify.API.Models.Requests
{
    public class ComplaintRequest
    {
        public Guid SubscriberId { get; set; }
        public string ReporterEmail { get; set; } = null!;
        public string Reason { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string NotificationSubject { get; set; } = null!;
        public string NotificationBody { get; set; } = null!;
    }
}
