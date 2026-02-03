namespace FertileNotify.Domain.Events
{
    public static class EventCatalog
    {
        private static readonly IReadOnlyCollection<EventType> _events =
        [
            EventType.SubscriberRegistered,
            EventType.TestForDevelop // for testing purposes
        ];

        public static bool IsSupported(EventType eventType)
        {
            return _events.Contains(eventType);
        }

        public static IReadOnlyCollection<EventType> All => _events;
    }
}
