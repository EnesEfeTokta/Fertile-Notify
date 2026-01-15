namespace FertileNotify.Domain.Rules
{
    public static class SubscriptionLimitRule
    {
        public const int MaxSubscriptionsPerUser = 10;

        public static bool CanAddSubscription(int currentSubscriptionCount)
        {
            return currentSubscriptionCount < MaxSubscriptionsPerUser;
        }
    }
}