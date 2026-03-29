namespace FertileNotify.Application.UseCases.ManageChannels
{
    public class ManageChannelsHandler: IRequestHandler<ManageChannelsCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public ManageChannelsHandler(ISubscriberRepository subscriberRepository, ISubscriptionRepository subscriptionRepository)
        {
            _subscriberRepository = subscriberRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public async Task<Unit> Handle(ManageChannelsCommand command, CancellationToken cancellationToken)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");
            
            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(subscriber.Id);

            var channel = NotificationChannel.From(command.Channel.Trim().ToLower());
            if (command.Enable)
                subscriber.EnableChannel(channel, subscription!.Plan);
            else
                subscriber.DisableChannel(channel);

            await _subscriberRepository.SaveAsync(subscriber);
            return Unit.Value;
        }
    }
}
