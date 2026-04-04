namespace FertileNotify.Application.Interfaces.Automation
{
    public interface IWorkflowScheduleService
    {
        Task SyncAsync(AutomationWorkflow workflow);
        Task RemoveAsync(Guid workflowId);
    }
}