namespace FertileNotify.Application.UseCases.Workflow
{
    public class CreateWorkflowNotificationHandler : IRequestHandler<CreateWorkflowNotificationCommand, Guid>
    {
        private readonly IAutomationRepository _automationRepository;
        private readonly IWorkflowScheduleService _workflowScheduleService;

        public CreateWorkflowNotificationHandler(
            IAutomationRepository automationRepository,
            IWorkflowScheduleService workflowScheduleService)
        {
            _automationRepository = automationRepository;
            _workflowScheduleService = workflowScheduleService;
        }

        public async Task<Guid> Handle(CreateWorkflowNotificationCommand request, CancellationToken cancellationToken)
        {
            var recipientGroups = BuildRecipientGroups(request.To);
            var content = NotificationContent.Create(request.Subject, request.Body);
            var eventType = EventType.From(request.EventType);

            var workflow = new AutomationWorkflow(
                request.SubscriberId,
                request.Name,
                request.Description,
                content,
                eventType,
                recipientGroups,
                request.EventTrigger?.Trim() ?? string.Empty,
                request.CronExpression,
                1,
                0);

            await _automationRepository.CreateAsync(workflow);
            await _automationRepository.SaveChangesAsync();

            await _workflowScheduleService.SyncAsync(workflow);

            return workflow.Id;
        }

        private static List<WorkflowRecipientGroup> BuildRecipientGroups(List<WorkflowRecipientGroupCommand> groups)
        {
            return groups
                .Where(group => !string.IsNullOrWhiteSpace(group.Channel))
                .GroupBy(group => group.Channel.Trim(), StringComparer.OrdinalIgnoreCase)
                .Select(group => new WorkflowRecipientGroup(
                    group.Key,
                    group
                        .SelectMany(x => x.Recipients)
                        .Where(recipient => !string.IsNullOrWhiteSpace(recipient))
                        .Select(recipient => recipient.Trim())
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .ToList()))
                .Where(group => group.Recipients.Count > 0)
                .ToList();
        }
    }
}
