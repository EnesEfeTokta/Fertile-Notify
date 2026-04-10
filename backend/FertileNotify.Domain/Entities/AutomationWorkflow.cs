using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;

namespace FertileNotify.Domain.Entities
{
    public class AutomationWorkflow
    {
        public Guid Id { get; private set; }
        public Guid SubscriberId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string EventType { get; private set; } = string.Empty;
        public List<WorkflowRecipientGroup> To { get; private set; } = new();
        public NotificationContent Content { get; private set; } = default!;
        public string EventTrigger { get; private set; } = string.Empty;
        public string CronExpression { get; private set; } = string.Empty;
        public int MaxRepeatCount { get; private set; } = 1;
        public int CurrentRepeatCount { get; private set; } = 0;
        public bool IsActive { get; private set; } = true;
        public DateTime CreatedAt { get; private set; }

        [NotMapped]
        public NotificationChannel? Channel =>
            string.IsNullOrWhiteSpace(To.FirstOrDefault()?.Channel)
                ? null
                : NotificationChannel.From(To[0].Channel);

        [NotMapped]
        public List<string> Recipients => To
            .SelectMany(group => group.Recipients)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

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
                BuildRecipientGroups(channel, recipients),
                eventTrigger,
                cronExpression,
                1,
                0)
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
            : this(
                subscriberId,
                name,
                description,
                content,
                eventType,
                BuildRecipientGroups(channel, recipients),
                eventTrigger,
                cronExpression,
                maxRepeatCount,
                currentRepeatCount)
        { }

        public AutomationWorkflow(
            Guid subscriberId,
            string name,
            string description,
            NotificationContent content,
            EventType eventType,
            List<WorkflowRecipientGroup> recipientGroups,
            string eventTrigger,
            string cronExpression,
            int maxRepeatCount,
            int currentRepeatCount)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Name = name;
            Description = description;
            Content = content;
            EventType = eventType.Name;
            To = recipientGroups;
            EventTrigger = eventTrigger;
            CronExpression = cronExpression;
            MaxRepeatCount = maxRepeatCount;
            CurrentRepeatCount = currentRepeatCount;
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
        public void UpdateEventType(EventType eventType) => EventType = eventType.Name;
        public void UpdateChannel(NotificationChannel channel)
        {
            if (To.Count == 0)
            {
                To = new List<WorkflowRecipientGroup> { new(channel.Name, new List<string>()) };
                return;
            }

            To = To.Select(group => new WorkflowRecipientGroup(channel.Name, group.Recipients)).ToList();
        }

        public void UpdateRecipients(List<string> recipients)
        {
            var channel = To.FirstOrDefault()?.Channel ?? NotificationChannel.Email.Name;
            To = new List<WorkflowRecipientGroup> { new(channel, recipients) };
        }

        public void UpdateRecipientGroups(List<WorkflowRecipientGroup> recipientGroups) => To = recipientGroups;

        public void UpdateSchedule(string eventTrigger, string cronExpression)
        {
            EventTrigger = eventTrigger;
            CronExpression = cronExpression;
        }

        private static List<WorkflowRecipientGroup> BuildRecipientGroups(NotificationChannel channel, List<string> recipients)
        {
            return new List<WorkflowRecipientGroup>
            {
                new(channel.Name, recipients)
            };
        }
    }
}
