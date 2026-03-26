using System;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class AutomationWorkflow
    {
        public Guid Id { get; private set; }
        public Guid SubscriberId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public List<NotificationChannel> Channels { get; private set; } = new();
        public string EventTrigger { get; private set; } = string.Empty;
        public string CronExpression { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }

        private AutomationWorkflow() { }

        public AutomationWorkflow(Guid subscriberId, string name, string description)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Name = name;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void UpdateDetails(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
