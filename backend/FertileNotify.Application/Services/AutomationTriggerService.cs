using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Contracts;
using MassTransit;

namespace FertileNotify.Application.Services
{
    public class AutomationTriggerService
    {
        private readonly IAutomationRepository _automationRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public AutomationTriggerService(IAutomationRepository automationRepository, IPublishEndpoint publishEndpoint)
        {
            _automationRepository = automationRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<int> TriggerWorkflowsAsync(Guid subscriberId, string eventTrigger)
        {
            var workflows = await _automationRepository.GetByEventTriggerAsync(subscriberId, eventTrigger);
            if (workflows == null || workflows.Count == 0) return 0;

            int totalQueued = 0;

            foreach (var workflow in workflows)
            {
                foreach (var recipient in workflow.Recipients)
                {
                    var message = new ProcessNotificationMessage
                    {
                        SubscriberId = subscriberId,
                        WorkflowId = workflow.Id,
                        Recipient = recipient,
                        EventType = "Campaign",
                        Channel = workflow.Channel.Name,
                        Parameters = new Dictionary<string, string>(),
                        DirectSubject = workflow.Content.Subject,
                        DirectBody = workflow.Content.Body
                    };

                    await _publishEndpoint.Publish(message);
                    totalQueued++;
                }
            }

            return totalQueued;
        }
    }
}