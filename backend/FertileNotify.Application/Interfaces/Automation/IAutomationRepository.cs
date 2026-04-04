namespace FertileNotify.Application.Interfaces.Automation
{
    public interface IAutomationRepository
    {
        Task<AutomationWorkflow?> GetByIdAsync(Guid id);
        Task<List<AutomationWorkflow>> GetBySubscriberIdAsync(Guid subscriberId);
        Task<List<AutomationWorkflow>?> GetByEventTriggerAsync(Guid subscriberId, string eventTrigger);
        Task CreateAsync(AutomationWorkflow workflow);
        void Update(AutomationWorkflow workflow);
        Task UpdateAsync(AutomationWorkflow workflow);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
