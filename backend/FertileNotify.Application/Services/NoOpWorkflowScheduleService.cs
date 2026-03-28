using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Services
{
    public class NoOpWorkflowScheduleService : IWorkflowScheduleService
    {
        public Task SyncAsync(AutomationWorkflow workflow) => Task.CompletedTask;

        public Task RemoveAsync(Guid workflowId) => Task.CompletedTask;
    }
}