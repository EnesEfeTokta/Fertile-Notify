using FertileNotify.Application.Interfaces;
using MassTransit;
using Cronos;
using FertileNotify.Application.Contracts;

namespace FertileNotify.Application.Services
{
    public class AutomationSchedulerService
    {
        private readonly IAutomationRepository _automationRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public AutomationSchedulerService(IAutomationRepository automationRepository, IPublishEndpoint publishEndpoint)
        {
            _automationRepository = automationRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task TriggerWorkflow(Guid workflowId)
        {
            var workflow = await _automationRepository.GetByIdAsync(workflowId);
            if (workflow == null || !workflow.IsActive || workflow.CurrentRepeatCount >= workflow.MaxRepeatCount) return;

            await _publishEndpoint.Publish(new ProcessNotificationMessage
            {
                SubscriberId = workflow.SubscriberId,
                Recipient = string.Join(",", workflow.Recipients),
                EventType = workflow.EventTrigger,
                Channel = workflow.Channel.Name,
                Parameters = new Dictionary<string, string> { { "Subject", workflow.Content.Subject }, { "Body", workflow.Content.Body } }
            });

            workflow.IncrementRepeatCount();
            _automationRepository.Update(workflow);
            await _automationRepository.SaveChangesAsync();
        }

        public static DateTime? GetNextRun(string cronExpression)
        {
            try { return CronExpression.Parse(cronExpression).GetNextOccurrence(DateTime.UtcNow); }
            catch { return null; }
        }
    }
}