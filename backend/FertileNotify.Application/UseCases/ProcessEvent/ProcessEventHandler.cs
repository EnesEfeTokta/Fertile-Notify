using FertileNotify.Application.Interfaces;

namespace FertileNotify.Application.UseCases.ProcessEvent
{
    public class ProcessEventHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly INotificationSender _notificationSender;

        public ProcessEventHandler(
            ISubscriptionRepository subscriptionRepository,
            INotificationSender notificationSender
        )
        {
            _subscriptionRepository = subscriptionRepository;
            _notificationSender = notificationSender;
        }

        public async Task HandleAsync(ProcessEventCommand command)
        {
            var subscription =
                await _subscriptionRepository.GetByUserIdAsync(command.UserId)
                ?? throw new Exception("Subscription not found");

            subscription.EnsureCanSendNotification();

            await _notificationSender.SendAsync(
                command.EventType,
                command.Payload
            );

            subscription.IncreaseUsage();
            await _subscriptionRepository.SaveAsync(command.UserId, subscription);
        }
    }
}