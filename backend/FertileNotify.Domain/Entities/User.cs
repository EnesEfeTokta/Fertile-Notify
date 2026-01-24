using FertileNotify.Domain.Rules;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public EmailAddress Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }

        private readonly HashSet<NotificationChannel> _activeChannels = new();
        public IReadOnlyCollection<NotificationChannel> ActiveChannels => _activeChannels;

        private User()
        {
            Email = default!;
        }

        public User(EmailAddress email, PhoneNumber? phoneNumber)
        {
            Id = Guid.NewGuid();
            Email = email;
            PhoneNumber = phoneNumber;

            _activeChannels.Add(NotificationChannel.Email);
        }

        public void EnableChannel(NotificationChannel channel)
        {
            if (!_activeChannels.Contains(channel)) return;

            if (!ChannelPreferenceRule.CanAddChannel(_activeChannels.Count))
                throw new InvalidOperationException("Cannot add more notification channels.");

            if (channel == NotificationChannel.SMS && PhoneNumber == null)
                throw new InvalidOperationException("Phone number is required to enable SMS channel.");

            _activeChannels.Add(channel);
        }

        public void DisableChannel(NotificationChannel channel)
        {
            if (_activeChannels.Contains(channel))
            {
                _activeChannels.Remove(channel);
            }
        }
    }
}
