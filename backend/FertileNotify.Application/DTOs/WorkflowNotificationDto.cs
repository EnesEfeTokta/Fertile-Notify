namespace FertileNotify.Application.DTOs
{
    public class WorkflowNotificationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
        public string EventTrigger { get; set; } = string.Empty;
        public string CronExpression { get; set; } = string.Empty;
        public List<string> Recipients { get; set; } = new();
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }
}
