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
            var recipients = request.To.SelectMany(x => x.Recipients)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList(); ;

            var resolvedChannel = ResolveChannel(request.Channel, request.To);
            var channel = NotificationChannel.From(resolvedChannel);
            var content = NotificationContent.Create(request.Subject, request.Body);

            var workflow = new AutomationWorkflow(
                request.SubscriberId,
                request.Name,
                request.Description,
                content,
                channel,
                request.EventTrigger?.Trim() ?? string.Empty,
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
    }
}
