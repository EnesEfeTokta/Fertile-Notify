namespace FertileNotify.Domain.Events
{
    public static class EventCatalog
    {
        private static readonly IReadOnlyCollection<EventType> _events =
        [
            EventType.OrderCreated,
            EventType.UserRegistered
        ];

        public static bool IsSupported(EventType eventType)
        {
            return _events.Contains(eventType);
        }

        public static IReadOnlyCollection<EventType> All => _events;
    }
}
