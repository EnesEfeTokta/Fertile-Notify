namespace FertileNotify.Application.UseCases.Workflow
{
    public abstract class WorkflowHandlerBase
    {
        protected readonly IAutomationRepository _automationRepository;

        protected WorkflowHandlerBase(IAutomationRepository automationRepository)
        {
            _automationRepository = automationRepository;
        }

        protected async Task<AutomationWorkflow> GetOwnedWorkflowAsync(Guid subscriberId, Guid workflowId)
        {
            var workflow = await _automationRepository.GetByIdAsync(workflowId);
            if (workflow == null || workflow.SubscriberId != subscriberId)
            {
                throw new NotFoundException("Workflow not found.");
            }

            return workflow;
        }

        protected static WorkflowNotificationDto MapToDto(AutomationWorkflow workflow)
        {
            return new WorkflowNotificationDto
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Description = workflow.Description,
                Channel = workflow.Channel.Name,
                EventTrigger = workflow.EventTrigger,
                CronExpression = workflow.CronExpression,
                Recipients = workflow.Recipients,
                IsActive = workflow.IsActive,
                CreatedAt = workflow.CreatedAt,
                Subject = workflow.Content.Subject,
                Body = workflow.Content.Body
            };
        }
    }
}
