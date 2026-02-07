using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.Rules;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Domain.Entities
{
    public class Subscriber
    {
        public Guid Id { get; private set; }
        public CompanyName CompanyName { get; private set; }
        public Password Password { get; private set; }
        public EmailAddress Email { get; private set; }
        public PhoneNumber? PhoneNumber { get; private set; }
        public RefreshToken? RefreshToken { get; private set; }

        private readonly HashSet<NotificationChannel> _activeChannels = new();
        public IReadOnlyCollection<NotificationChannel> ActiveChannels => _activeChannels;

        private Subscriber()
        {
            CompanyName = default!;
            Password = default!;
            Email = default!;
        }

        public Subscriber(
            CompanyName companyName, 
            Password password, 
            EmailAddress email, 
            PhoneNumber? phoneNumber
        )
        {
            Id = Guid.NewGuid();
            CompanyName = companyName;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;

            _activeChannels.Add(NotificationChannel.Email);
        }

        public void UpdateCompanyName(CompanyName companyName) => CompanyName = companyName;

        public void UpdatePassword(Password password) => Password = password;

        public void SetRefreshToken(RefreshToken refreshToken) => RefreshToken = refreshToken;

        public void UpdateContactInfo(EmailAddress email, PhoneNumber? phoneNumber)
        {
            if (phoneNumber == null && _activeChannels.Contains(NotificationChannel.SMS))
                throw new BusinessRuleException("Cannot remove phone number while SMS channel is active.", "CHN_1306");

            Email = email;
            PhoneNumber = phoneNumber;
        }

        public void EnableChannel(NotificationChannel channel, SubscriptionPlan plan)
        {
            if (_activeChannels.Contains(channel)) return;

            if (!SubscriptionChannelPolicy.CanUseChannel(plan, channel))
                throw new BusinessRuleException($"Your plan ({plan}) does not support the {channel.Name} channel.", "CMP_1507");

            if (!ChannelPreferenceRule.CanAddChannel(_activeChannels.Count))
                throw new BusinessRuleException("Cannot add more notification channels.", "CMP_1506");

            if (channel == NotificationChannel.SMS && PhoneNumber == null)
                throw new BusinessRuleException("Phone number is required to enable SMS channel.", "USR_1103");

            _activeChannels.Add(channel);
        }

        public void DisableChannel(NotificationChannel channel)
        {
            if (_activeChannels.Contains(channel))
                _activeChannels.Remove(channel);
        }
    }
}
