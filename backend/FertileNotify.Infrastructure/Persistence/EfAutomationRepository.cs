using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FertileNotify.Infrastructure.Persistence
{
    public class EfAutomationRepository : IAutomationRepository
    {
        private readonly ApplicationDbContext _context;

        public EfAutomationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(AutomationWorkflow workflow) 
            => await _context.AutomationWorkflows.AddAsync(workflow);

        public async Task DeleteAsync(Guid id)
        {
            var workflow = await _context.AutomationWorkflows.FindAsync(id);
            if (workflow != null)
            {
                _context.AutomationWorkflows.Remove(workflow);
            }
        }

        public async Task<AutomationWorkflow?> GetByIdAsync(Guid id) 
            => await _context.AutomationWorkflows.FindAsync(id);

        public async Task<List<AutomationWorkflow>> GetBySubscriberIdAsync(Guid subscriberId)
            => await _context.AutomationWorkflows.Where(w => w.SubscriberId == subscriberId).ToListAsync();

        public async Task<List<AutomationWorkflow>?> GetByEventTriggerAsync(Guid subscriberId, string eventTrigger)
            => await _context.AutomationWorkflows
                .Where(w => w.SubscriberId == subscriberId
                    && w.IsActive
                    && w.EventTrigger.ToLower() == eventTrigger.ToLower())
                .ToListAsync();

        public void Update(AutomationWorkflow workflow) => _context.AutomationWorkflows.Update(workflow);

        public Task UpdateAsync(AutomationWorkflow workflow)
        {
            Update(workflow);
            return Task.CompletedTask;
        }
        
        public async Task SaveChangesAsync() 
            => await _context.SaveChangesAsync();
    }
}
