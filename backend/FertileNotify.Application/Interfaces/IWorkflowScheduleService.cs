using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface IWorkflowScheduleService
    {
        Task SyncAsync(AutomationWorkflow workflow);
        Task RemoveAsync(Guid workflowId);
    }
}