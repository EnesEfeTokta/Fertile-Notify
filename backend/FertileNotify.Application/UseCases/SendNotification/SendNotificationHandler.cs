namespace FertileNotify.Application.UseCases.SendNotification
{
    public class SendNotificationHandler : IRequestHandler<SendNotificationCommand, int>
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly ILogger<SendNotificationHandler> _logger;

        public SendNotificationHandler(
            IPublishEndpoint publishEndpoint,
            IBlacklistRepository blacklistRepository,
            ILogger<SendNotificationHandler> logger)
        {
            _publishEndpoint = publishEndpoint;
            _blacklistRepository = blacklistRepository;
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
                        context.Headers.Set("notification-priority", priority);
                    }, cancellationToken);
                    totalQueued++;
                }
            }

            return totalQueued;
        }
    }
}
