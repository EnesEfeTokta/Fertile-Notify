namespace FertileNotify.Domain.Rules
{
    public static class SubscriptionRule
    {
        public static void EnsureIsActive(DateTime expiresAt)
        {
            if (expiresAt < DateTime.UtcNow)
                throw new InvalidOperationException("Subscription has expired.");
        }

        public static void EnsureCanSendNotification(int limit, int used)
        {
            if (used >= limit)
                throw new InvalidOperationException("Notification limit reached.");
        }
    }
}