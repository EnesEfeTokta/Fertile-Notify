namespace FertileNotify.Application.UseCases.Templates
{
    public class TemplateQueryItem
    {
        public string EventType { get; set; } = string.Empty;
        public string Channel { get; set; } = string.Empty;
    }

    public class TemplateResponseItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public EventType EventType { get; set; } = default!;
        public NotificationChannel Channel { get; set; } = default!;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;

        public static TemplateResponseItem FromTemplate(NotificationTemplate template)
            => new()
            {
                Id = template.Id,
                Name = template.Name,
                Description = template.Description,
                EventType = template.EventType,
                Channel = template.Channel,
                Subject = template.Content.Subject,
                Body = template.Content.Body,
                Source = template.SubscriberId == null ? "Default" : "Custom"
            };
    }
}