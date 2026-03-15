using FertileNotify.Application.Contracts;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Application.UseCases.SendNotification
{
    public class SendNotificationHandler
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;
        private readonly IEnumerable<INotificationSender> _senders;
        private readonly TemplateEngine _templateEngine;
        private readonly IStatsRepository _statsRepository;
        private readonly ISecurityService _securityService;
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
            IStatsRepository statsRepository,
            ISecurityService securityService,
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
            _statsRepository = statsRepository;
            _securityService = securityService;
            _logger = logger;
        }

        public async Task<int> HandleAsync(SendNotificationCommand command)
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
                    });
                    totalQueued++;
                }
            }

            return totalQueued;
        }

        public async Task ProcessNotificationAsync(ProcessNotificationMessage message)
        {
            var channel = NotificationChannel.From(message.Channel);
            var eventType = EventType.From(message.EventType);

            _logger.LogInformation("[HANDLING] Event: {Event}, Channel: {Channel}, Sub: {SubId}",
                eventType.Name, channel.Name, message.SubscriberId);

            var (subscriber, subscription) = await GetAndValidateEntities(message.SubscriberId, eventType, channel);

            var unsubscribeToken = _securityService.GenerateUnsubscribeToken(message.Recipient, message.SubscriberId);
            if (!message.Parameters.ContainsKey("UnsubscribeLink"))
            {
                message.Parameters["UnsubscribeLink"] = 
                    $"http://fertile-notify.enesefetokta.shop/unsubscribe?recipient={message.Recipient}&subId={message.SubscriberId}&token={unsubscribeToken}";
            }

            var (subject, body) = await PrepareContent(message.SubscriberId, eventType, channel, message.Parameters);

            var sender = GetSender(channel);
            var channelSetting = await _subscriberChannelRepository.GetSettingAsync(message.SubscriberId, channel);

            bool isSuccess = await sender.SendAsync(
                message.SubscriberId,
                message.Recipient,
                eventType,
                subject,
                body,
                channelSetting?.Settings);

            await FinalizeProcess(message.SubscriberId, eventType, channel, subscription, isSuccess, message.Recipient);
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

            subscription.EnsureCanSendNotification();

            return (subscriber, subscription);
        }

        private async Task<(string Subject, string Body)> PrepareContent(Guid subscriberId, EventType eventType, NotificationChannel channel, Dictionary<string, string> parameters)
        {
            var template = await _templateRepository.GetTemplateAsync(eventType, channel, subscriberId)
                ?? throw new NotFoundException($"No template for {eventType.Name} on {channel.Name}");

            var subject = _templateEngine.Render(template.SubjectTemplate, channel, parameters);
            var body = _templateEngine.Render(template.BodyTemplate, channel, parameters);

            return (subject, body);
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

        private async Task FinalizeProcess(Guid subscriberId, EventType eventType, NotificationChannel channel, Subscription subscription, bool isSuccess, string recipient)
        {
            await _statsRepository.IncrementAsync(
                subscriberId,
                channel,
                eventType,
                isSuccess
            );

            if (isSuccess)
            {
                subscription.IncreaseUsage();
                await _subscriptionRepository.SaveAsync(subscriberId, subscription);

                _logger.LogInformation("[SUCCESS] Notification processed. Usage: {Used}/{Limit}",
                    subscription.UsedThisMonth, subscription.MonthlyLimit);
            }
            else
            {
                _logger.LogWarning("[FAILED] Notification could not be sent to {Recipient}", recipient);
            }
        }
    }
}
