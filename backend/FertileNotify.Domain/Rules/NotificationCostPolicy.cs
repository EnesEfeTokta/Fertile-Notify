using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Rules
{
    public static class NotificationCostPolicy
    {
        public static int GetCost(NotificationChannel channel)
        {
            if (channel == NotificationChannel.Email) return 1;
            if (channel == NotificationChannel.Console) return 0;
            if (channel == NotificationChannel.WebPush) return 1;
            if (channel == NotificationChannel.Slack) return 1;
            if (channel == NotificationChannel.Telegram) return 1;
            if (channel == NotificationChannel.Discord) return 1;
            if (channel == NotificationChannel.Slack) return 1;
            if (channel == NotificationChannel.FirebasePush) return 1;
            if (channel == NotificationChannel.Webhook) return 1;
            if (channel == NotificationChannel.WhatsApp) return 5;
            if (channel == NotificationChannel.SMS) return 10;

            return 1;
        }
    }
}
