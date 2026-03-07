using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class SubscriberDailyStats
    {
        public Guid Id { get; private set; }
        public Guid SubscriberId { get; private set; }
        public DateTime Date { get; private set; }
        public NotificationChannel Channel { get; private set; } = default!;
        public EventType EventType { get; private set; } = default!;
        public int SuccessCount { get; private set; }
        public int FailedCount { get; private set; }

        private SubscriberDailyStats() { }

        public SubscriberDailyStats(Guid subscriberId, NotificationChannel channel, EventType eventType)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Date = DateTime.UtcNow.Date;
            Channel = channel;
            EventType = eventType;
            SuccessCount = 0;
            FailedCount = 0;
        }

        public void IncreaseSuccess() => SuccessCount++;
        public void IncreaseFailure() => FailedCount++;
    }
}