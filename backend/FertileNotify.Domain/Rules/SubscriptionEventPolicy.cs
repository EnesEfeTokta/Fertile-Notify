using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Events;

namespace FertileNotify.Domain.Rules
{
    public static class SubscriptionEventPolicy
    {
        public static IReadOnlyCollection<EventType> GetAllowedEvents(SubscriptionPlan plan)
        {
            return plan switch
            {
                SubscriptionPlan.Free =>
                [
                    EventType.SubscriberRegistered,
                    EventType.PasswordReset,
                    EventType.EmailVerified,
                    EventType.LoginAlert,

                    EventType.OrderCancelled,
                    EventType.OrderShipped,
                    EventType.OrderDelivered,
                    EventType.OrderCreated,
                    EventType.PaymentFailed,

                    EventType.Campaign,
                    EventType.MonthlyNewsletter,
                    EventType.SupportTicketUpdated
                ],

                SubscriptionPlan.Pro =>
                [
                    EventType.SubscriberRegistered,
                    EventType.PasswordReset,
                    EventType.EmailVerified,
                    EventType.LoginAlert,

                    EventType.OrderCancelled,
                    EventType.OrderShipped,
                    EventType.OrderDelivered,
                    EventType.OrderCreated,
                    EventType.PaymentFailed,

                    EventType.Campaign,
                    EventType.MonthlyNewsletter,
                    EventType.SupportTicketUpdated
                ],

                SubscriptionPlan.Enterprise =>
                    EventCatalog.All,

                _ => []
            };
        }
    }
}
