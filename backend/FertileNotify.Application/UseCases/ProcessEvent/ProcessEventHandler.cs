using FertileNotify.Application.Interfaces;

namespace FertileNotify.Application.UseCases.ProcessEvent
{
    public class ProcessEventHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEnumerable<INotificationSender> _senders;

        public ProcessEventHandler(
            ISubscriptionRepository subscriptionRepository,
            IUserRepository userRepository,
            IEnumerable<INotificationSender> senders
        )
        {
            _subscriptionRepository = subscriptionRepository;
            _userRepository = userRepository;
            _senders = senders;
        }

        public async Task HandleAsync(ProcessEventCommand command)
        {
            var user = 
                await _userRepository.GetByIdAsync(command.UserId) 
                ?? throw new Exception("User not found");

            var subscription =
                await _subscriptionRepository.GetByUserIdAsync(command.UserId)
                ?? throw new Exception("Subscription not found");

            if (!subscription.CanHandle(command.EventType))
                return;

            subscription.EnsureCanSendNotification();

            foreach (var channel in user.ActiveChannels)
            {
                var sender = _senders.FirstOrDefault(s => s.Equals(channel));

                if (sender == null) continue;

                await sender.SendAsync(command.EventType.Name, command.Payload);
            }

            subscription.IncreaseUsage();
            await _subscriptionRepository.SaveAsync(command.UserId, subscription);
        }
    }
}