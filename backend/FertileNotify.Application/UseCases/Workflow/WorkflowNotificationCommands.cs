namespace FertileNotify.Application.UseCases.Workflow
{
    public class WorkflowRecipientGroupCommand
    {
        public string Channel { get; set; } = string.Empty;
        public List<string> Recipients { get; set; } = new();
    }

    public class CreateWorkflowNotificationCommand
    {
        public Guid SubscriberId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Channel { get; set; }
        public string EventTrigger { get; set; } = string.Empty;
        public string CronExpression { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<WorkflowRecipientGroupCommand> To { get; set; } = new();
    }

    public class UpdateWorkflowNotificationCommand
    {
        public Guid SubscriberId { get; set; }
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Channel { get; set; }
        public string? EventTrigger { get; set; }
        public string? CronExpression { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public List<WorkflowRecipientGroupCommand>? To { get; set; }
    }

    public class WorkflowNotificationByIdCommand
    {
        public Guid SubscriberId { get; set; }
        public Guid Id { get; set; }
    }

    public class TriggerWorkflowNotificationsCommand
    {
        public Guid SubscriberId { get; set; }
        public string EventTrigger { get; set; } = string.Empty;
    }
}
