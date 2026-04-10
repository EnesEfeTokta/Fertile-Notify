namespace FertileNotify.Application.UseCases.Notifications
{
    public class ProcessNotificationMessageHandler : IRequestHandler<ProcessNotificationMessage>
    {
        private readonly IEnumerable<INotificationSender> _senders;
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;
        private readonly INotificationLogService _notificationLogService;
        private readonly ILogger<ProcessNotificationMessageHandler> _logger;

        public ProcessNotificationMessageHandler(
            IEnumerable<INotificationSender> senders,
            ISubscriberChannelRepository subscriberChannelRepository,
            INotificationLogService notificationLogService,
            ILogger<ProcessNotificationMessageHandler> logger)
        {
            _senders = senders;
            _subscriberChannelRepository = subscriberChannelRepository;
            _notificationLogService = notificationLogService;
            _logger = logger;
        }

        public async Task Handle(ProcessNotificationMessage message, CancellationToken cancellationToken)
        {
            var channel = NotificationChannel.From(message.Channel);
            var sender = _senders.FirstOrDefault(candidate => candidate.Channel.Equals(channel))
                ?? throw new InvalidOperationException($"No notification sender registered for channel '{message.Channel}'.");

            if (string.IsNullOrWhiteSpace(message.DirectSubject) || string.IsNullOrWhiteSpace(message.DirectBody))
            {
                await _notificationLogService.LogRejectedAsync(message, null, "Notification content is missing.");
                return;
            }

            var content = NotificationContent.Create(message.DirectSubject, message.DirectBody);
            var channelSetting = await _subscriberChannelRepository.GetSettingAsync(message.SubscriberId, channel);
            var providerSettings = channelSetting?.Settings;

            try
            {
                var sent = await sender.SendAsync(
                    message.SubscriberId,
                    message.Recipient,
                    EventType.From(message.EventType),
                    content,
                    providerSettings);

                if (sent)
                {
                    await _notificationLogService.LogSuccessAsync(message, content);
                }
                else
                {
                    await _notificationLogService.LogFailureAsync(message, content, $"Sender '{sender.Channel.Name}' returned false.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed processing notification for {Recipient} on {Channel}.", message.Recipient, message.Channel);
                await _notificationLogService.LogFailureAsync(message, content, ex.Message);
            }

        }
    }
}