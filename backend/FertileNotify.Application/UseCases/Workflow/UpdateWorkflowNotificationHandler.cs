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

            if (!string.IsNullOrWhiteSpace(request.Subject) || !string.IsNullOrWhiteSpace(request.Body))
            {
                var currentContent = workflow.Content;
                var updatedContent = NotificationContent.Create(
                    request.Subject ?? currentContent.Subject,
                    request.Body ?? currentContent.Body);
                workflow.UpdateContent(updatedContent);
            }

            if (!string.IsNullOrWhiteSpace(request.Channel))
            {
                workflow.UpdateChannel(NotificationChannel.From(request.Channel));
            }

            if (request.To != null && request.To.Count > 0)
            {
                var recipients = CollectRecipients(request.To);
                if (recipients.Count > 0)
                {
                    workflow.UpdateRecipients(recipients);
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
