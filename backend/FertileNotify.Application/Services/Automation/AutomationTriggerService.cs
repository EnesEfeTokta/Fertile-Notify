namespace FertileNotify.Application.Services.Automation
{
    public class AutomationTriggerService
    {
        private readonly IAutomationRepository _automationRepository;
        private readonly INotificationDispatchService _dispatchService;
        private readonly ILogger<AutomationTriggerService> _logger;

        public AutomationTriggerService(
            IAutomationRepository automationRepository,
            INotificationDispatchService dispatchService,
            ILogger<AutomationTriggerService> logger)
        {
            _automationRepository = automationRepository;
            _dispatchService = dispatchService;
            _logger = logger;
        }

        public async Task<int> TriggerWorkflowsAsync(Guid subscriberId, string eventTrigger)
        {
            var workflows = await _automationRepository.GetByEventTriggerAsync(subscriberId, eventTrigger);
            if (workflows == null || workflows.Count == 0)
            {
                _logger.LogWarning("No active workflow found for SubscriberId={SubscriberId} and EventTrigger={EventTrigger}.", subscriberId, eventTrigger);
                return 0;
            }

            int totalQueued = 0;

            foreach (var workflow in workflows)
            {
                foreach (var group in workflow.To)
                {
                    foreach (var recipient in group.Recipients)
                    {
                        var message = new ProcessNotificationMessage
                        {
                            SubscriberId = subscriberId,
                            WorkflowId = workflow.Id,
                            Recipient = recipient,
                            EventType = workflow.EventType,
                            Channel = group.Channel,
                            Parameters = new Dictionary<string, string>(),
                            DirectSubject = workflow.Content.Subject,
                            DirectBody = workflow.Content.Body
                        };

                        await _dispatchService.QueueAsync(message, "workflow-trigger");
                        totalQueued++;
                    }
                }
            }

            return totalQueued;
        }
    }
}