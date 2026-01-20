namespace FertileNotify.Domain.Events
{
    public sealed class EventType
    {
        public string Name { get; }

        private EventType(string name)
        {
            Name = name;
        }

        public static readonly EventType OrderCreated = new("OrderCreated");
        public static readonly EventType UserRegistered = new("UserRegistered");

        public static EventType From(string name)
        {
            return name switch
            {
                "OrderCreated" => OrderCreated,
                "UserRegistered" => UserRegistered,
                _ => throw new Exception("Unknown event type")
            };
        }

        public override string ToString() => Name;

        public override bool Equals(object? obj)
            => obj is EventType other && Name == other.Name;

        public override int GetHashCode()
            => Name.GetHashCode();
    }
}