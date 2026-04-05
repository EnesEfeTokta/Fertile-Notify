namespace FertileNotify.Application.DTOs.Subscribers
{
    public class ExportDataDto
    {
        public Guid DataOwnerSubscriberId { get; set; }
        public DateTime ExportedAtUtc { get; set; }
        public SubscriberDto Subscriber { get; set; } = new();
        public List<NotificationLogDto> NotificationLogs { get; set; } = new();
        public List<NotificationTemplateDto> NotificationTemplates { get; set; } = new();
        public List<ApiKeyDto> ApiKeys { get; set; } = new();
        public List<ChannelConfigurationDto> ChannelConfigurations { get; set; } = new();
        public List<WorkflowNotificationDto> WorkflowNotifications { get; set; } = new();
        public List<BlacklistEntryDto> BlacklistEntries { get; set; } = new();
        public List<NotificationComplaintDto> NotificationComplaints { get; set; } = new();
    }
}
