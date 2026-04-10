namespace FertileNotify.Application.UseCases.Workflow
{
    public class UpdateWorkflowNotificationHandler : WorkflowHandlerBase, IRequestHandler<UpdateWorkflowNotificationCommand, Unit>
    {
        private readonly IWorkflowScheduleService _workflowScheduleService;

        public UpdateWorkflowNotificationHandler(
            IAutomationRepository automationRepository,
            IWorkflowScheduleService workflowScheduleService) : base(automationRepository)
        {
            _workflowScheduleService = workflowScheduleService;
        }

        public async Task<Unit> Handle(UpdateWorkflowNotificationCommand request, CancellationToken cancellationToken)
        {
            var workflow = await GetOwnedWorkflowAsync(request.SubscriberId, request.Id);

            if (!string.IsNullOrWhiteSpace(request.Name) || !string.IsNullOrWhiteSpace(request.Description))
            {
                workflow.UpdateDetails(request.Name ?? workflow.Name, request.Description ?? workflow.Description);
            }

            if (!string.IsNullOrWhiteSpace(request.EventType))
            {
                var eventType = EventType.From(request.EventType);
                workflow.UpdateEventType(eventType);
            }

            if (!string.IsNullOrWhiteSpace(request.Subject) || !string.IsNullOrWhiteSpace(request.Body))
            {
                var currentContent = workflow.Content;
                var updatedContent = NotificationContent.Create(
                    request.Subject ?? currentContent.Subject,
                    request.Body ?? currentContent.Body);
                workflow.UpdateContent(updatedContent);
            }

            if (request.To != null && request.To.Count > 0)
            {
                var recipientGroups = BuildRecipientGroups(request.To);
                if (recipientGroups.Count > 0)
                {
                    workflow.UpdateRecipientGroups(recipientGroups);
                }
            }

            // Handle schedule changes
            var cronChanged = false;
            if (!string.IsNullOrWhiteSpace(request.EventTrigger) || !string.IsNullOrWhiteSpace(request.CronExpression))
            {
                var oldCron = workflow.CronExpression;
                workflow.UpdateSchedule(
                    request.EventTrigger ?? workflow.EventTrigger,
                    request.CronExpression ?? workflow.CronExpression);
                
                cronChanged = oldCron != workflow.CronExpression;
            }

            _automationRepository.Update(workflow);
            await _automationRepository.SaveChangesAsync();

            if (cronChanged)
            {
                await _workflowScheduleService.SyncAsync(workflow);
            }

            return Unit.Value;
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
