namespace FertileNotify.Application.DTOs.Notifications
{
    public class NotificationComplaintDto
    {
        public Guid Id { get; set; }
        public string ReporterEmail { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}