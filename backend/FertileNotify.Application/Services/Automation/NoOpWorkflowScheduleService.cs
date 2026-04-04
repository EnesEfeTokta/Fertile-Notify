namespace FertileNotify.Application.Services.Automation
{
    public class NoOpWorkflowScheduleService : IWorkflowScheduleService
    {
        public Task SyncAsync(AutomationWorkflow workflow) => Task.CompletedTask;

        public Task RemoveAsync(Guid workflowId) => Task.CompletedTask;
    }
}