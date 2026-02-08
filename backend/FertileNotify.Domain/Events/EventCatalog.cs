namespace FertileNotify.Domain.Events
{
    public static class EventCatalog
    {
        private static readonly IReadOnlyCollection<EventType> _events =
        [
            // Auth
            EventType.SubscriberRegistered,
            EventType.PasswordReset,
            EventType.EmailVerified,
            EventType.LoginAlert,
            EventType.AccountLocked,

            // E-Commerce
            EventType.OrderCreated,
            EventType.OrderShipped,
            EventType.OrderDelivered,
            EventType.OrderCancelled,
            EventType.PaymentFailed,
            EventType.SubscriptionRenewed,

            // General
            EventType.Campaign,
            EventType.MonthlyNewsletter,
            EventType.SupportTicketUpdated
        ];

        public static bool IsSupported(EventType eventType) => _events.Contains(eventType);
        public static IReadOnlyCollection<EventType> All => _events;
    }
}