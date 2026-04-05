namespace FertileNotify.Application.UseCases.DeleteAccount
{
    public class DeleteAccountHandler: IRequestHandler<DeleteAccountCommand, Unit>
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;
        private readonly INotificationLogRepository _notificationLogRepository;
        private readonly IStatsRepository _statsRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IAutomationRepository _automationRepository;
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly INotificationComplaintRepository _notificationComplaintRepository;
        private readonly IApiKeyRepository _apiKeyRepository;
        
        private readonly ILogger<DeleteAccountHandler> _logger;

        public DeleteAccountHandler(
            ISubscriberRepository subscriberRepository,
            ISubscriptionRepository subscriptionRepository,
            ISubscriberChannelRepository subscriberChannelRepository,
            INotificationLogRepository notificationLogRepository,
            IStatsRepository statsRepository,
            ITemplateRepository templateRepository,
            IAutomationRepository automationRepository,
            IBlacklistRepository blacklistRepository,
            INotificationComplaintRepository notificationComplaintRepository,
            IApiKeyRepository apiKeyRepository,
            ILogger<DeleteAccountHandler> logger)
        {
            _subscriberRepository = subscriberRepository;
            _subscriptionRepository = subscriptionRepository;
            _subscriberChannelRepository = subscriberChannelRepository;
            _notificationLogRepository = notificationLogRepository;
            _statsRepository = statsRepository;
            _templateRepository = templateRepository;
            _automationRepository = automationRepository;
            _blacklistRepository = blacklistRepository;
            _notificationComplaintRepository = notificationComplaintRepository;
            _apiKeyRepository = apiKeyRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteAccountCommand command, CancellationToken ct)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            await _subscriptionRepository.DeleteBySubscriberIdAsync(command.SubscriberId);
            await _subscriberChannelRepository.DeleteBySubscriberIdAsync(command.SubscriberId);
            await _notificationLogRepository.DeleteBySubscriberIdAsync(command.SubscriberId);
            await _statsRepository.DeleteBySubscriberIdAsync(command.SubscriberId);
            await _templateRepository.DeleteBySubscriberIdAsync(command.SubscriberId);
            await _automationRepository.DeleteBySubscriberIdAsync(command.SubscriberId);
            await _blacklistRepository.DeleteBySubscriberIdAsync(command.SubscriberId);
            await _notificationComplaintRepository.DeleteBySubscriberIdAsync(command.SubscriberId);
            await _apiKeyRepository.DeleteBySubscriberIdAsync(command.SubscriberId);
            
            await _subscriberRepository.DeleteAsync(subscriber.Id);

            _logger.LogInformation("Subscriber with ID {SubscriberId} has been deleted.", command.SubscriberId);
            return Unit.Value;
        }
    }
}
