namespace FertileNotify.Domain.Events
{
    public sealed class EventType
    {
        public string Name { get; }

        private EventType(string name)
        {
            Name = name;
        }

        // --- AUTH & ACCOUNT ---
        public static readonly EventType SubscriberRegistered = new("SubscriberRegistered");
        public static readonly EventType PasswordReset = new("PasswordReset");
        public static readonly EventType EmailVerified = new("EmailVerified");
        public static readonly EventType LoginAlert = new("LoginAlert");
        public static readonly EventType AccountLocked = new("AccountLocked");

        // --- E-COMMERCE ---
        public static readonly EventType OrderCreated = new("OrderCreated");
        public static readonly EventType OrderShipped = new("OrderShipped");
        public static readonly EventType OrderDelivered = new("OrderDelivered");
        public static readonly EventType OrderCancelled = new("OrderCancelled");
        public static readonly EventType PaymentFailed = new("PaymentFailed");
        public static readonly EventType SubscriptionRenewed = new("SubscriptionRenewed");

        // --- GENERAL & MARKETING ---
        public static readonly EventType Campaign = new("Campaign");
        public static readonly EventType MonthlyNewsletter = new("MonthlyNewsletter");
        public static readonly EventType SupportTicketUpdated = new("SupportTicketUpdated");

        // --- WORKFLOW & AUTOMATION ---
        public static readonly EventType WorkflowTriggered = new("WorkflowTriggered");

        // --- FOR TESTING PURPOSES ---
        public static readonly EventType TestForDevelop = new("TestForDevelop");

        public static EventType From(string name)
        {
            return name switch
            {
                // Auth
                "SubscriberRegistered" => SubscriberRegistered,
                "PasswordReset" => PasswordReset,
                "EmailVerified" => EmailVerified,
                "LoginAlert" => LoginAlert,
                "AccountLocked" => AccountLocked,

                // E-Commerce
                "OrderCreated" => OrderCreated,
                "OrderShipped" => OrderShipped,
                "OrderDelivered" => OrderDelivered,
                "OrderCancelled" => OrderCancelled,
                "PaymentFailed" => PaymentFailed,
                "SubscriptionRenewed" => SubscriptionRenewed,

                // General
                "Campaign" => Campaign,
                "MonthlyNewsletter" => MonthlyNewsletter,
                "SupportTicketUpdated" => SupportTicketUpdated,

                // For Test
                "TestForDevelop" => TestForDevelop,

                _ => throw new ArgumentException($"Unknown event type: {name}")
            };
        }

        public override string ToString() => Name;
        public override bool Equals(object? obj) => obj is EventType other && Name == other.Name;
        public override int GetHashCode() => Name.GetHashCode();

        public byte GetPriority()
        {
            return Name switch
            {
                // Critical Level
                "SubscriberRegistered" => 10,
                "PasswordReset" => 10,
                "AccountLocked" => 9,
                "PaymentFailed" => 9,

                // Important Level
                "LoginAlert" => 8,
                "EmailVerified" => 7,
                "OrderCreated" => 7,
                "OrderCancelled" => 6,

                // Normal Level
                "SubscriptionRenewed" => 5,
                "OrderShipped" => 5,
                "SupportTicketUpdated" => 5,
                "OrderDelivered" => 4,

                // Low Level
                "Campaign" => 1,
                "MonthlyNewsletter" => 0,

                // For Test
                "TestForDevelop" => 10,

                // Default
                _ => 1
            };
        }
    }
}