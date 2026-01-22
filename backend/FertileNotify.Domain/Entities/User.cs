using FertileNotify.Domain.Rules;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public EmailAddress Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }

        private readonly HashSet<NotificationChannel> _activeChannel = new();
        public IReadOnlyCollection<NotificationChannel> ActiveChannels => _activeChannel;

        private User()
        {
            Email = default!;
        }

        public User(EmailAddress email, PhoneNumber? phoneNumber)
        {
            Id = Guid.NewGuid();
            Email = email;
            PhoneNumber = phoneNumber;

            _activeChannel.Add(NotificationChannel.Console);
        }

        public void EnableChannel(NotificationChannel channel)
        {
            if (!_activeChannel.Contains(channel)) return;

            if (!ChannelPreferenceRule.CanAddChannel(_activeChannel.Count))
                throw new InvalidOperationException("Cannot add more notification channels.");

            if (channel == NotificationChannel.SMS && PhoneNumber == null)
                throw new InvalidOperationException("Phone number is required to enable SMS channel.");

            _activeChannel.Add(channel);
        }

        public void DisableChannel(NotificationChannel channel)
        {
            if (_activeChannel.Contains(channel))
            {
                _activeChannel.Remove(channel);
            }
        }
    }
}
