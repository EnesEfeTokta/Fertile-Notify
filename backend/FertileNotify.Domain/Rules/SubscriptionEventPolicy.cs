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
                    EventType.UserRegistered
                ],

                SubscriptionPlan.Pro =>
                [
                    EventType.UserRegistered,
                    EventType.OrderCreated
                ],

                SubscriptionPlan.Enterprise =>
                    EventCatalog.All,

                _ => []
            };
        }
    }
}
