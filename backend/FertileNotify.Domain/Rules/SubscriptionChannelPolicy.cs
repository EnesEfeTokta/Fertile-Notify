using FertileNotify.Domain.Enums;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Rules
{
    public static class SubscriptionChannelPolicy
    {
        public static bool CanUseChannel(SubscriptionPlan plan, NotificationChannel channel)
        {
            if (plan == SubscriptionPlan.Enterprise) return true;

            if (plan == SubscriptionPlan.Pro) return true;

            if (plan == SubscriptionPlan.Free)
                return channel == NotificationChannel.Email || channel == NotificationChannel.Console;

            return false;
        }
    }
}