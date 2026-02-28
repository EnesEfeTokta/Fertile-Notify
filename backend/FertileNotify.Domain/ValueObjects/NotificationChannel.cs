namespace FertileNotify.Domain.ValueObjects
{
    public class NotificationChannel : IEquatable<NotificationChannel>
    {
        public string Name { get; }

        private NotificationChannel(string name)
        {
            Name = name;
        }

        public static readonly NotificationChannel Console = new("console");
        public static readonly NotificationChannel SMS = new("sms");
        public static readonly NotificationChannel Email = new("email");
        public static readonly NotificationChannel Telegram = new("telegram");
        public static readonly NotificationChannel Discord = new("discord");
        public static readonly NotificationChannel WhatsApp = new("whatsapp");

        public static NotificationChannel From(string value)
        {
            return value switch
            {
                "console" => Console,
                "sms" => SMS,
                "email" => Email,
                "telegram" => Telegram,
                "discord" => Discord,
                "whatsapp" => WhatsApp,
                _ => throw new ArgumentException($"Unknown notification channel: {value}")
            };
        }

        public bool Equals(NotificationChannel? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Name == other.Name;
        }

        public override bool Equals(object? obj) => Equals(obj as NotificationChannel);

        public override string ToString() => Name;

        public override int GetHashCode() => Name.GetHashCode();
    }
}