using Cronos;

namespace FertileNotify.Application.Services.Automation
{
    public class AutomationSchedulerService
    {
        private readonly IAutomationRepository _automationRepository;
        private readonly INotificationDispatchService _dispatchService;

        public AutomationSchedulerService(IAutomationRepository automationRepository, INotificationDispatchService dispatchService)
        {
            _automationRepository = automationRepository;
            _dispatchService = dispatchService;
        }

        public async Task TriggerWorkflow(Guid workflowId)
        {
            var workflow = await _automationRepository.GetByIdAsync(workflowId);
            if (workflow == null || !workflow.IsActive || workflow.CurrentRepeatCount >= workflow.MaxRepeatCount) return;

            foreach (var recipient in workflow.Recipients)
            {
                var message = new ProcessNotificationMessage
                {
                    SubscriberId = workflow.SubscriberId,
                    WorkflowId = workflow.Id,
                    Recipient = recipient,
                    EventType = "Campaign",
                    Channel = workflow.Channel.Name,
                    Parameters = new Dictionary<string, string>(),
                    DirectSubject = workflow.Content.Subject,
                    DirectBody = workflow.Content.Body
                };

                await _dispatchService.QueueAsync(message, "workflow-scheduler");
            }

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