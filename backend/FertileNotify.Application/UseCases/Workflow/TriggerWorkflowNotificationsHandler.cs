namespace FertileNotify.Application.UseCases.Workflow
{
    public class TriggerWorkflowNotificationsHandler : IRequestHandler<TriggerWorkflowNotificationsCommand, int>
    {
        private readonly AutomationTriggerService _automationTriggerService;

        public TriggerWorkflowNotificationsHandler(AutomationTriggerService automationTriggerService)
        {
            _automationTriggerService = automationTriggerService;
        }

        public async Task<int> Handle(TriggerWorkflowNotificationsCommand request, CancellationToken cancellationToken)
            => await _automationTriggerService.TriggerWorkflowsAsync(request.SubscriberId, request.EventTrigger.Trim());
    }
}
