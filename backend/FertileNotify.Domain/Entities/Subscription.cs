using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Rules;

namespace FertileNotify.Domain.Entities
{
    public class Subscription
    {
        public Guid Id { get; private set; }
        public SubscriptionPlan Plan { get; private set; }
        public int MonthlyLimit { get; private set; }
        public int UsedThisMonth { get; private set; }
        public DateTime ExpiresAt { get; private set; }

        public Subscription(SubscriptionPlan plan, int monthlyLimit, DateTime expiresAt)
        {
            Id = Guid.NewGuid();
            Plan = plan;
            MonthlyLimit = monthlyLimit;
            UsedThisMonth = 0;
            ExpiresAt = expiresAt;
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
    }
}