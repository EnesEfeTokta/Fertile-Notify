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

        public override string ToString() => Name;
    }
}