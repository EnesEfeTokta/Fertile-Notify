using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.Rules;

namespace FertileNotify.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; private set; }
        public Guid SubscriberId { get; private set; }
        public SubscriptionPlan Plan { get; private set; }
        public int MonthlyLimit { get; private set; }
        public int UsedThisMonth { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        private readonly HashSet<EventType> _allowedEvents;
        public IReadOnlyCollection<EventType> AllowedEvents => _allowedEvents;

        private Subscription() 
        {
            _allowedEvents = new HashSet<EventType>();
        }

        private Subscription(
            Guid subscriberId,
            SubscriptionPlan plan,
            int monthlyLimit,
            DateTime expiresAt,
            IEnumerable<EventType> allowedEvents
        )
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Plan = plan;
            MonthlyLimit = monthlyLimit;
            UsedThisMonth = 0;
            ExpiresAt = expiresAt;
            _allowedEvents = new HashSet<EventType>(allowedEvents);
        }

        public static Subscription Create(Guid userId, SubscriptionPlan plan)
        {
            int limit = plan switch
            {
                SubscriptionPlan.Free => 10,
                SubscriptionPlan.Pro => 1000,
                SubscriptionPlan.Enterprise => 10000,
                _ => 0
            };

            var allowedEvents = SubscriptionEventPolicy.GetAllowedEvents(plan);
            var expiresAt = DateTime.UtcNow.AddMonths(1);

            return new Subscription(userId, plan, limit, expiresAt, allowedEvents);
        }

        public int GetRemainingMonthlyLimit()
        {
            if (ExpiresAt < DateTime.UtcNow) return 0;
            return Math.Max(0, MonthlyLimit - UsedThisMonth);
        }

        public bool TryUseMonthlyLimit(int cost)
        {
            if (GetRemainingMonthlyLimit() >= cost)
            {
                UsedThisMonth += cost;
                return true;
            }
            return false;
        }

        public bool CanHandle(EventType eventType) => _allowedEvents.Contains(eventType);
    }
}