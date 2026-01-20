using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.Rules;

namespace FertileNotify.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }

        public SubscriptionPlan Plan { get; private set; }
        public int MonthlyLimit { get; private set; }
        public int UsedThisMonth { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public Subscription(
            Guid userId, 
            SubscriptionPlan plan, 
            int monthlyLimit, 
            DateTime expiresAt,
            IEnumerable<EventType> allowedEvents
        )
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Plan = plan;
            MonthlyLimit = monthlyLimit;
            UsedThisMonth = 0;
            ExpiresAt = expiresAt;

            _allowedEvents = new HashSet<EventType>(allowedEvents);
        }

        public void EnsureCanSendNotification()
        {
            SubscriptionRule.EnsureIsActive(ExpiresAt);
            SubscriptionRule.EnsureCanSendNotification(MonthlyLimit, UsedThisMonth);
        }

        public void IncreaseUsage()
        {
            UsedThisMonth++;
        }

        private readonly HashSet<EventType> _allowedEvents;
        public IReadOnlyCollection<EventType> AllowedEvents => _allowedEvents;

        public bool CanHandle(EventType eventType)
        {
            return _allowedEvents.Contains(eventType);
        }
    }
}