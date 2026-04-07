using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class AutomationWorkflow
    {
        public Guid Id { get; private set; }
        public Guid SubscriberId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string EventType { get; private set; } = string.Empty;
        public NotificationChannel Channel { get; private set; } = default!;
        public NotificationContent Content { get; private set; } = default!;
        public string EventTrigger { get; private set; } = string.Empty;
        public string CronExpression { get; private set; } = string.Empty;
        public int MaxRepeatCount { get; private set; } = 1;
        public int CurrentRepeatCount { get; private set; } = 0;
        public List<string> Recipients { get; private set; } = new();
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }

        private AutomationWorkflow() { }

        public AutomationWorkflow(
            Guid subscriberId,
            string name,
            string description,
            NotificationContent content,
            EventType eventType,
            NotificationChannel channel,
            string eventTrigger,
            string cronExpression,
            List<string> recipients)
            : this(
                subscriberId,
                name,
                description,
                content,
                eventType,
                channel,
                eventTrigger,
                cronExpression,
                1,
                0,
                recipients)
        { }

        public AutomationWorkflow(
            Guid subscriberId,
            string name,
            string description,
            NotificationContent content,
            EventType eventType,
            NotificationChannel channel,
            string eventTrigger,
            string cronExpression,
            int maxRepeatCount,
            int currentRepeatCount,
            List<string> recipients)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Name = name;
            Description = description;
            Content = content;
            EventType = eventType.Name;
            Channel = channel;
            EventTrigger = eventTrigger;
            CronExpression = cronExpression;
            MaxRepeatCount = maxRepeatCount;
            CurrentRepeatCount = currentRepeatCount;
            Recipients = recipients;
            CreatedAt = DateTime.UtcNow;
        }

        public void Activate() => IsActive = true;
        public void Deactivate() => IsActive = false;

        public void IncrementRepeatCount() => CurrentRepeatCount++;

        public void UpdateDetails(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public void UpdateContent(NotificationContent content) => Content = content;
        public void UpdateChannel(NotificationChannel channel) => Channel = channel;
        public void UpdateEventType(EventType eventType) => EventType = eventType.Name;
        public void UpdateRecipients(List<string> recipients) => Recipients = recipients;

        public void UpdateSchedule(string eventTrigger, string cronExpression)
        {
            EventTrigger = eventTrigger;
            CronExpression = cronExpression;
        }
    }
}
