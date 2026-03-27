using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.Interfaces
{
    public interface IAutomationRepository
    {
        Task<AutomationWorkflow?> GetByIdAsync(Guid id);
        Task<List<AutomationWorkflow>> GetBySubscriberIdAsync(Guid subscriberId);
        Task<List<AutomationWorkflow>?> GetByEventTriggerAsync(Guid subscriberId, string eventTrigger);
        Task CreateAsync(AutomationWorkflow workflow);
        void UpdateAsync(AutomationWorkflow workflow);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
