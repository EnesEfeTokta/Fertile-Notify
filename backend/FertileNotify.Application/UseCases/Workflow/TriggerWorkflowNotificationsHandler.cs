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
        {
            if (string.IsNullOrWhiteSpace(request.EventTrigger))
            {
                throw new BusinessRuleException("eventTrigger cannot be empty.");
            }

            return await _automationTriggerService.TriggerWorkflowsAsync(request.SubscriberId, request.EventTrigger.Trim());
        }
    }
}
