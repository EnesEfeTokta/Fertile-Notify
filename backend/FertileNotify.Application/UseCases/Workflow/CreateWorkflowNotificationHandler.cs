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
            var recipients = CollectRecipients(request.To);
            if (recipients.Count == 0)
            {
                throw new BusinessRuleException("At least one recipient is required.");
            }

            if (string.IsNullOrWhiteSpace(request.EventTrigger) && string.IsNullOrWhiteSpace(request.CronExpression))
            {
                throw new BusinessRuleException("Either EventTrigger or CronExpression is required.");
            }

            var resolvedChannel = ResolveChannel(request.Channel, request.To);
            var channel = NotificationChannel.From(resolvedChannel);
            var content = NotificationContent.Create(request.Subject, request.Body);

            var workflow = new AutomationWorkflow(
                request.SubscriberId,
                request.Name,
                request.Description,
                content,
                channel,
                request.EventTrigger.Trim(),
                request.CronExpression,
                1,
                0,
                recipients);

            await _automationRepository.CreateAsync(workflow);
            await _automationRepository.SaveChangesAsync();

            await _workflowScheduleService.SyncAsync(workflow);

            return workflow.Id;
        }

        private static string ResolveChannel(string? commandChannel, List<WorkflowRecipientGroupCommand> recipients)
        {
            if (!string.IsNullOrWhiteSpace(commandChannel))
            {
                return commandChannel;
            }

            var firstChannel = recipients.FirstOrDefault()?.Channel;
            if (string.IsNullOrWhiteSpace(firstChannel))
            {
                throw new BusinessRuleException("Channel is required.");
            }

            return firstChannel;
        }

        private static List<string> CollectRecipients(List<WorkflowRecipientGroupCommand> groups)
        {
            return groups
                .SelectMany(x => x.Recipients)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
