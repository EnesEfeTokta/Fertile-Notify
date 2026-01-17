using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;

namespace FertileNotify.Application.UseCases.ProcessEvent
{
    public class ProcessEventHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly INotificationRepository _notificationRepository;

        public ProcessEventHandler(
            ISubscriptionRepository subscriptionRepository,
            INotificationRepository notificationRepository
        )
        {
            _subscriptionRepository = subscriptionRepository;
            _notificationRepository = notificationRepository;
        }

        public async Task HandleAsync(ProcessEventCommand command)
        {
            var subscription = await _subscriptionRepository.GetByIdAsync(command.UserId);
            
            if (subscription is null) 
                throw new Exception("Subscription not found");

            subscription.EnsureCanSendNotification();

            var notification = new Notification(command.Title, command.Message);
            await _notificationRepository.SendAsync(notification);

            subscription.IncreaseUsage();

            await _subscriptionRepository.SaveAsync(subscription);
        }
    }
}