namespace FertileNotify.Domain.ValueObjects
{
    public class NotificationChannel : IEquatable<NotificationChannel>
    {
        public string Name { get; }

        private NotificationChannel(string name)
        {
            Name = name;
        }

        public static readonly NotificationChannel Console = new("Console");
        public static readonly NotificationChannel SMS = new("SMS");
        public static readonly NotificationChannel Email = new("Email");

        public static NotificationChannel From(string value)
        {
            return value switch
            {
                "Console" => Console,
                "SMS" => SMS,
                "Email" => Email,
                _ => throw new ArgumentException($"Unknown notification channel: {value}")
            };
        }

        public bool Equals(NotificationChannel? other) => Equals(other as NotificationChannel);

        public override bool Equals(object? obj) => Equals(obj as NotificationChannel);

        public override string ToString() => Name;

        public override int GetHashCode() => Name.GetHashCode();
    }
}