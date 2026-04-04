namespace FertileNotify.Application.UseCases.SendNotification
{
    public class SendNotificationHandler : IRequestHandler<SendNotificationCommand, int>
    {
        private readonly INotificationDispatchService _dispatchService;
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly ILogger<SendNotificationHandler> _logger;

        public SendNotificationHandler(
            INotificationDispatchService dispatchService,
            IBlacklistRepository blacklistRepository,
            ILogger<SendNotificationHandler> logger)
        {
            _dispatchService = dispatchService;
            _blacklistRepository = blacklistRepository;
            _logger = logger;
        }

        public async Task<int> Handle(SendNotificationCommand command, CancellationToken cancellationToken)
        {
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

                    var message = new ProcessNotificationMessage
                    {
                        SubscriberId = command.SubscriberId,
                        Recipient = recipientAddress,
                        EventType = command.EventType,
                        Channel = channel.Name,
                        Parameters = command.Parameters
                    };

                    await _dispatchService.QueueAsync(message, "api-send", cancellationToken);
                    totalQueued++;
                }
            }

            return totalQueued;
        }
    }
}
