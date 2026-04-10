namespace FertileNotify.Application.UseCases.Workflow
{
    public class WorkflowRecipientGroupCommand
    {
        public string Channel { get; set; } = string.Empty;
        public List<string> Recipients { get; set; } = new();
    }

    public class CreateWorkflowNotificationCommand : IRequest<Guid>
    {
        public Guid SubscriberId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public string EventTrigger { get; set; } = string.Empty;
        public string CronExpression { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<WorkflowRecipientGroupCommand> To { get; set; } = new();
    }

    public class UpdateWorkflowNotificationCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? EventType { get; set; }
        public string? EventTrigger { get; set; }
        public string? CronExpression { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
        public List<WorkflowRecipientGroupCommand>? To { get; set; }
    }

    public class GetWorkflowNotificationQuery : IRequest<WorkflowNotificationDto>
    {
        public Guid SubscriberId { get; set; }
        public Guid Id { get; set; }
    }

    public class ListWorkflowNotificationsQuery : IRequest<List<WorkflowNotificationDto>>
    {
        public Guid SubscriberId { get; set; }
    }

    public class DeleteWorkflowNotificationCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public Guid Id { get; set; }
    }

    public class ActivateWorkflowNotificationCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public Guid Id { get; set; }
    }

    public class DeactivateWorkflowNotificationCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public Guid Id { get; set; }
    }

    public class WorkflowNotificationByIdCommand
    {
        public Guid SubscriberId { get; set; }
        public Guid Id { get; set; }
    }

    public class TriggerWorkflowNotificationsCommand : IRequest<int>
    {
        public Guid SubscriberId { get; set; }
        public string EventTrigger { get; set; } = string.Empty;
    }
}
