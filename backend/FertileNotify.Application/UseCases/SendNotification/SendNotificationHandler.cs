namespace FertileNotify.Application.UseCases.SendNotification
{
    public class SendNotificationHandler : IRequestHandler<SendNotificationCommand, int>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;
        private readonly IEnumerable<INotificationSender> _senders;
        private readonly TemplateEngine _templateEngine;
        private readonly ISecurityService _securityService;
        private readonly INotificationLogService _logService;
        private readonly ILogger<SendNotificationHandler> _logger;
        private readonly Dictionary<NotificationChannel, INotificationSender> _senderMap;

        public SendNotificationHandler(
            IPublishEndpoint publishEndpoint,
            IBlacklistRepository blacklistRepository,
            ISubscriptionRepository subscriptionRepository,
            ISubscriberRepository subscriberRepository,
            IEnumerable<INotificationSender> senders,
            ITemplateRepository templateRepository,
            ISubscriberChannelRepository subscriberChannelRepository,
            TemplateEngine templateEngine,
            ISecurityService securityService,
            INotificationLogService logService,
            ILogger<SendNotificationHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _blacklistRepository = blacklistRepository;
            _subscriptionRepository = subscriptionRepository;
            _subscriberRepository = subscriberRepository;
            _senders = senders;
            _senderMap = senders.ToDictionary(s => s.Channel);
            _templateRepository = templateRepository;
            _subscriberChannelRepository = subscriberChannelRepository;
            _templateEngine = templateEngine;
            _securityService = securityService;
            _logService = logService;
            _logger = logger;
        }

        public async Task<int> Handle(SendNotificationCommand command, CancellationToken cancellationToken)
        {
            var eventType = EventType.From(command.EventType);
            byte priority = eventType.GetPriority();

            var allAddresses = command.To.SelectMany(g => g.Recipients).Distinct().ToList();
            var blacklistedItems = await _blacklistRepository.GetForRecipientsAsync(command.SubscriberId, allAddresses);

            int totalQueued = 0;

            foreach (var group in command.To)
            {
                var channel = NotificationChannel.From(group.Channel);
                foreach (var recipientAddress in group.Recipients)
                {
                    var blacklistEntry = blacklistedItems.FirstOrDefault(b => b.RecipientAddress == recipientAddress);

                    if (blacklistEntry != null)
                    {
                        if (blacklistEntry.UnwantedChannels.Count == 0 ||
                            blacklistEntry.UnwantedChannels.Contains(channel))
                        {
                            _logger.LogInformation("Notification skipped for {Recipient} on {Channel} due to blacklist.", recipientAddress, channel);
                            continue;
                        }
                    }

                    await _publishEndpoint.Publish<ProcessNotificationMessage>(new
                    {
                        SubscriberId = command.SubscriberId,
                        Recipient = recipientAddress,
                        EventType = command.EventType,
                        Channel = channel.Name,
                        Parameters = command.Parameters
                    }, context => {
                        context.SetPriority(priority);
                    }, cancellationToken);
                    totalQueued++;
                }
            }

            return totalQueued;
        }
    }
}
