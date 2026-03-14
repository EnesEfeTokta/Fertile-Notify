using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Application.UseCases.ProcessEvent
{
    public class ProcessEventHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;
        private readonly Dictionary<NotificationChannel, INotificationSender> _senderMap;
        private readonly TemplateEngine _templateEngine;
        private readonly IStatsRepository _statsRepository;
        private readonly ISecurityService _securityService;
        private readonly ILogger<ProcessEventHandler> _logger;

        public ProcessEventHandler(
            ISubscriptionRepository subscriptionRepository,
            ISubscriberRepository subscriberRepository,
            IEnumerable<INotificationSender> senders,
            ITemplateRepository templateRepository,
            ISubscriberChannelRepository subscriberChannelRepository,
            TemplateEngine templateEngine,
            IStatsRepository statsRepository,
            ILogger<ProcessEventHandler> logger,
            ISecurityService securityService)
        {
            _subscriptionRepository = subscriptionRepository;
            _subscriberRepository = subscriberRepository;
            _senderMap = senders.ToDictionary(s => s.Channel);
            _templateRepository = templateRepository;
            _subscriberChannelRepository = subscriberChannelRepository;
            _templateEngine = templateEngine;
            _statsRepository = statsRepository;
            _logger = logger;
            _securityService = securityService;
        }

        public async Task HandleAsync(ProcessEventCommand command)
        {
            _logger.LogInformation("[HANDLING] Event: {Event}, Channel: {Channel}, Sub: {SubId}",
                command.EventType.Name, command.Channel.Name, command.SubscriberId);

            var (subscriber, subscription) = await GetAndValidateEntities(command);

            var unsubscribeToken = _securityService.GenerateUnsubscribeToken(command.Recipient, command.SubscriberId);
            if (!command.Parameters.ContainsKey("UnsubscribeLink"))
            {
                command.Parameters["UnsubscribeLink"] = 
                    $"http://fertile-notify.enesefetokta.shop/unsubscribe?recipient={command.Recipient}&subId={command.SubscriberId}&token={unsubscribeToken}";
            }

            var (subject, body) = await PrepareContent(command);

            var sender = GetSender(command.Channel);
            var channelSetting = await _subscriberChannelRepository.GetSettingAsync(command.SubscriberId, command.Channel);

            bool isSuccess = await sender.SendAsync(
                command.SubscriberId,
                command.Recipient,
                command.EventType,
                subject,
                body,
                channelSetting?.Settings);

            await FinalizeProcess(command, subscription, isSuccess);
        }

        private async Task<(Subscriber, Subscription)> GetAndValidateEntities(ProcessEventCommand command)
        {
            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found");

            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscription not found");

            if (!subscription.CanHandle(command.EventType))
                throw new BusinessRuleException($"Plan does not support event: {command.EventType.Name}");

            if (!subscriber.ActiveChannels.Contains(command.Channel))
                throw new BusinessRuleException($"Channel {command.Channel.Name} is not enabled for this subscriber.");

            subscription.EnsureCanSendNotification();

            return (subscriber, subscription);
        }

        private async Task<(string Subject, string Body)> PrepareContent(ProcessEventCommand command)
        {
            var template = await _templateRepository.GetTemplateAsync(command.EventType, command.Channel, command.SubscriberId)
                ?? throw new NotFoundException($"No template for {command.EventType.Name} on {command.Channel.Name}");

            var subject = _templateEngine.Render(template.SubjectTemplate, command.Channel, command.Parameters);
            var body = _templateEngine.Render(template.BodyTemplate, command.Channel, command.Parameters);

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

        private async Task FinalizeProcess(ProcessEventCommand command, Subscription subscription, bool isSuccess)
        {
            await _statsRepository.IncrementAsync(
                command.SubscriberId,
                command.Channel,
                command.EventType,
                isSuccess
            );

            if (isSuccess)
            {
                subscription.IncreaseUsage();
                await _subscriptionRepository.SaveAsync(command.SubscriberId, subscription);

                _logger.LogInformation("[SUCCESS] Notification processed. Usage: {Used}/{Limit}",
                    subscription.UsedThisMonth, subscription.MonthlyLimit);
            }
            else
            {
                _logger.LogWarning("[FAILED] Notification could not be sent to {Recipient}", command.Recipient);
            }
        }
    }
}