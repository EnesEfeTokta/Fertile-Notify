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
        public int ExtraCredits { get; private set; } = 0;

        private readonly HashSet<NotificationChannel> _activeChannels = new();
        public IReadOnlyCollection<NotificationChannel> ActiveChannels => _activeChannels;

        private Subscriber()
        {
            CompanyName = default!;
            Password = default!;
            Email = default!;
        }

        public Subscriber(CompanyName companyName, Password password, EmailAddress email, PhoneNumber? phoneNumber)
        {
            Id = Guid.NewGuid();
            CompanyName = companyName;
            Password = password;
            Email = email;
            PhoneNumber = phoneNumber;

            _activeChannels.Add(NotificationChannel.Email);
        }

        public Subscriber WithCompanyName(CompanyName companyName)
        {
            CompanyName = companyName;
            return this;
        }

        public Subscriber WithEmail(EmailAddress email)
        {
            Email = email;
            return this;
        }

        public Subscriber WithPhoneNumber(PhoneNumber? phoneNumber)
        {
            if (phoneNumber == null && _activeChannels.Contains(NotificationChannel.SMS))
                throw new BusinessRuleException("Cannot remove phone number while SMS channel is active.", "CHN_1306");

            PhoneNumber = phoneNumber;
            return this;
        }

        public Subscriber WithPassword(Password password)
        {
            Password = password;
            return this;
        }

        public Subscriber SetRefreshToken(RefreshToken refreshToken) 
        {
            RefreshToken = refreshToken;
            return this;
        }

        public Subscriber AddCredits(int amount)
        {
            if (amount <= 0) throw new ArgumentException("Amount must be positive");
            ExtraCredits += amount;
            return this;
        }

        public bool TryUseExtraCredit(int amount = 1)
        {
            if (ExtraCredits >= amount)
            {
                ExtraCredits -= amount;
                return true;
            }
            return false;
        }

        public Subscriber EnableChannel(NotificationChannel channel, SubscriptionPlan plan)
        {
            if (_activeChannels.Contains(channel)) return this;

            if (!SubscriptionChannelPolicy.CanUseChannel(plan, channel))
                throw new BusinessRuleException($"Your plan ({plan}) does not support the {channel.Name} channel.", "CMP_1507");

            if (!ChannelPreferenceRule.CanAddChannel(_activeChannels.Count))
                throw new BusinessRuleException("Cannot add more notification channels.", "CMP_1506");

            if (channel == NotificationChannel.SMS && PhoneNumber == null)
                throw new BusinessRuleException("Phone number is required to enable SMS channel.", "USR_1103");

            _activeChannels.Add(channel);
            return this;
        }

        public Subscriber DisableChannel(NotificationChannel channel)
        {
            if (_activeChannels.Contains(channel))
                _activeChannels.Remove(channel);
            return this;
        }
    }
}