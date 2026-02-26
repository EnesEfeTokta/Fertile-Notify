using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class SubscriberChannelSetting
    {
        public Guid Id { get; private set; }
        public Guid SubscriberId { get; private set; }
        public NotificationChannel Channel { get; private set; } = default!;

        private readonly Dictionary<string, string> _settings = new();
        public IReadOnlyDictionary<string, string> Settings => _settings;

        private SubscriberChannelSetting() { }

        public SubscriberChannelSetting(Guid subscriberId, NotificationChannel channel, Dictionary<string, string> settings)
        {
            Id = Guid.NewGuid();
            SubscriberId = subscriberId;
            Channel = channel;
            foreach (var s in settings) _settings.Add(s.Key, s.Value);
        }

        public void UpdateSettings(Dictionary<string, string> newSettings)
        {
            _settings.Clear();
            foreach (var s in newSettings) _settings.Add(s.Key, s.Value);
        }
    }
}