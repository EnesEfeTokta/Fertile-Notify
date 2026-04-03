namespace FertileNotify.Application.UseCases.SendNotification
{
    public class ProcessNotificationHandler : IRequestHandler<ProcessNotificationMessage>
    {
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;
        private readonly IEnumerable<INotificationSender> _senders;
        private readonly TemplateEngine _templateEngine;
        private readonly ISecurityService _securityService;
        private readonly INotificationLogService _logService;
        private readonly ILogger<ProcessNotificationHandler> _logger;
        private readonly Dictionary<NotificationChannel, INotificationSender> _senderMap;

        public ProcessNotificationHandler(
            IBlacklistRepository blacklistRepository,
            ISubscriptionRepository subscriptionRepository,
            ISubscriberRepository subscriberRepository,
            IEnumerable<INotificationSender> senders,
            ITemplateRepository templateRepository,
            ISubscriberChannelRepository subscriberChannelRepository,
            TemplateEngine templateEngine,
            ISecurityService securityService,
            INotificationLogService logService,
            ILogger<ProcessNotificationHandler> logger)
        {
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

        public async Task Handle(ProcessNotificationMessage message, CancellationToken cancellationToken)
        {
            var channel = NotificationChannel.From(message.Channel);
            var eventType = EventType.From(message.EventType);
            NotificationContent? notification = null;

            try
            {
                var (subscriber, subscription) = await GetAndValidateEntities(message.SubscriberId, eventType, channel);

                var unsubscribeToken = _securityService.GenerateUnsubscribeToken(message.Recipient, message.SubscriberId);
                if (!message.Parameters.ContainsKey("RecipientsManagerLink"))
                    message.Parameters["RecipientsManagerLink"] = 
                        $"http://fertile-notify.enesefetokta.shop/recipients-manager?recipient={message.Recipient}&subId={message.SubscriberId}&token={unsubscribeToken}";

                var content = ResolveContent(message);
                if (content == null)
                {
                    content = await PrepareContent(message.SubscriberId, eventType, channel, message.Parameters);
                }

                if (string.IsNullOrWhiteSpace(content.Value.Subject) || string.IsNullOrWhiteSpace(content.Value.Body))
                {
                    throw new BusinessRuleException(
                        $"Template content cannot be empty for event '{eventType.Name}' on channel '{channel.Name}'.");
                }

                notification = NotificationContent.Create(content.Value.Subject, content.Value.Body);

                var sender = GetSender(channel);
                var channelSetting = await _subscriberChannelRepository.GetSettingAsync(message.SubscriberId, channel);

                bool isSuccess = await sender.SendAsync(
                    message.SubscriberId,
                    message.Recipient,
                    eventType,
                    notification,
                    channelSetting?.Settings);

                if (isSuccess)
                {
                    await ChargeCreditsAsync(subscriber, subscription, channel);
                    await _logService.LogSuccessAsync(message, notification);
                }
                else
                {
                    await _logService.LogFailureAsync(message, notification, "Provider rejected the message.");
                }
            }
            catch (BusinessRuleException ex)
            {
                await _logService.LogRejectedAsync(message, notification, ex.Message);
            }
            catch (Exception ex)
            {
                await _logService.LogFailureAsync(message, notification, ex.Message);
                _logger.LogError(ex, "Error processing notification for {Recipient}", message.Recipient);
                throw;
            }
        }

        private async Task ChargeCreditsAsync(Subscriber subscriber, Subscription subscription, NotificationChannel channel)
        {
            int cost = NotificationCostPolicy.GetCost(channel);

            if (subscription.TryUseMonthlyLimit(cost))
            {
                await _subscriptionRepository.SaveAsync(subscriber.Id, subscription);
                _logger.LogInformation("Used {Cost} credits from Monthly Plan. Remaining: {Remaining}", cost, subscription.GetRemainingMonthlyLimit());
            }
            else if (subscriber.TryUseExtraCredit(cost))
            {
                await _subscriberRepository.SaveAsync(subscriber);
                _logger.LogInformation("Used {Cost} credits from Extra Wallet. Remaining: {Remaining}", cost, subscriber.ExtraCredits);
            }
        }

        private async Task<(Subscriber, Subscription)> GetAndValidateEntities(Guid subscriberId, EventType eventType, NotificationChannel channel)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(subscriberId)
                ?? throw new NotFoundException("Subscriber not found");

            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(subscriberId)
                ?? throw new NotFoundException("Subscription not found");

            if (!subscription.CanHandle(eventType))
                throw new BusinessRuleException($"Plan does not support event: {eventType.Name}");

            if (!subscriber.ActiveChannels.Contains(channel))
                throw new BusinessRuleException($"Channel {channel.Name} is not enabled for this subscriber.");

            return (subscriber, subscription);
        }

        private async Task<(string Subject, string Body)> PrepareContent(Guid subscriberId, EventType eventType, NotificationChannel channel, Dictionary<string, string> parameters)
        {
            var template = await _templateRepository.GetTemplateAsync(eventType, channel, subscriberId)
                ?? throw new NotFoundException($"No template for {eventType.Name} on {channel.Name}");

            var subject = _templateEngine.Render(template.Content.Subject, channel, parameters);
            var body = _templateEngine.Render(template.Content.Body, channel, parameters);

            return (subject, body);
        }

        private static (string Subject, string Body)? ResolveContent(ProcessNotificationMessage message)
        {
            if (string.IsNullOrWhiteSpace(message.DirectSubject) || string.IsNullOrWhiteSpace(message.DirectBody))
            {
                return null;
            }

            return (message.DirectSubject, message.DirectBody);
        }

        private INotificationSender GetSender(NotificationChannel channel)
        {
            if (!_senderMap.TryGetValue(channel, out var sender))
            {
                _logger.LogError("Sender not implemented for: {Channel}", channel.Name);
                throw new Exception($"System Error: No sender for {channel.Name}");
            }
            return sender;
        }
    }
}
