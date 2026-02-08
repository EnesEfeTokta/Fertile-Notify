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
        public static readonly EventType SubscriberRegistered = new("SubscriberRegistered"); // Yeni üye
        public static readonly EventType PasswordReset = new("PasswordReset"); // Şifre sıfırlama
        public static readonly EventType EmailVerified = new("EmailVerified"); // E-posta doğrulandı
        public static readonly EventType LoginAlert = new("LoginAlert"); // Yeni cihazdan giriş yapıldı
        public static readonly EventType AccountLocked = new("AccountLocked"); // Hesap kilitlendi

        // --- E-COMMERCE ---
        public static readonly EventType OrderCreated = new("OrderCreated"); // Sipariş alındı
        public static readonly EventType OrderShipped = new("OrderShipped"); // Kargo yola çıktı
        public static readonly EventType OrderDelivered = new("OrderDelivered"); // Teslim edildi
        public static readonly EventType OrderCancelled = new("OrderCancelled"); // İptal edildi
        public static readonly EventType PaymentFailed = new("PaymentFailed"); // Ödeme başarısız
        public static readonly EventType SubscriptionRenewed = new("SubscriptionRenewed"); // Abonelik yenilendi (Firma müşterisi için)

        // --- GENERAL & MARKETING ---
        public static readonly EventType Campaign = new("Campaign"); // Genel kampanya
        public static readonly EventType MonthlyNewsletter = new("MonthlyNewsletter"); // Bülten
        public static readonly EventType SupportTicketUpdated = new("SupportTicketUpdated"); // Destek talebi güncellendi
        public static readonly EventType TestForDevelop = new("TestForDevelop"); // Geliştirme testi

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

                _ => throw new Exception($"Unknown event type: {name}")
            };
        }

        public override string ToString() => Name;
        public override bool Equals(object? obj) => obj is EventType other && Name == other.Name;
        public override int GetHashCode() => Name.GetHashCode();
    }
}