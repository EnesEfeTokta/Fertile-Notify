using FertileNotify.Application.Contracts;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.Rules;
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

            string? subject = null;
            string? body = null;
            Subscriber? subscriber = null;
            Subscription? subscription = null;

            try
            {
                var validated = await GetAndValidateEntities(message.SubscriberId, eventType, channel);
                subscriber = validated.Item1;
                subscription = validated.Item2;

                var unsubscribeToken = _securityService.GenerateUnsubscribeToken(message.Recipient, message.SubscriberId);
                if (!message.Parameters.ContainsKey("UnsubscriberLink"))
                    message.Parameters["UnsubscriberLink"] = 
                        $"http://fertile-notify.enesefetokta.shop/unsubscribe?recipient={message.Recipient}&subId={message.SubscriberId}&token={unsubscribeToken}";

                var content = await PrepareContent(message.SubscriberId, eventType, channel, message.Parameters);
                subject = content.Subject;
                body = content.Body;

                var sender = GetSender(channel);
                var channelSetting = await _subscriberChannelRepository.GetSettingAsync(message.SubscriberId, channel);

                bool isSuccess = await sender.SendAsync(
                    message.SubscriberId,
                    message.Recipient,
                    eventType,
                    subject,
                    body,
                    channelSetting?.Settings);

                if (isSuccess)
                {
                    await ChargeCreditsAsync(subscriber, subscription, channel);
                    await _logService.LogSuccessAsync(message, subject, body);
                }
                else
                {
                    await _logService.LogFailureAsync(message, subject, body, "Provider rejected the message.");
                }
            }
            catch (BusinessRuleException ex)
            {
                await _logService.LogRejectedAsync(message, subject, body ,ex.Message);
            }
            catch (Exception ex)
            {
                await _logService.LogFailureAsync(message, subject, body, ex.Message);
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

            var subject = _templateEngine.Render(template.Subject, channel, parameters);
            var body = _templateEngine.Render(template.Body, channel, parameters);

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
    }
}
