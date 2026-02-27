using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;
using FertileNotify.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Application.UseCases.ProcessEvent
{
    public class ProcessEventHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly IEnumerable<INotificationSender> _senders;
        private readonly ITemplateRepository _templateRepository;
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;
        private readonly TemplateEngine templateEngine;
        private readonly ILogger<ProcessEventHandler> _logger;

        public ProcessEventHandler(
            ISubscriptionRepository subscriptionRepository,
            ISubscriberRepository subscriberRepository,
            IEnumerable<INotificationSender> senders,
            ITemplateRepository templateRepository,
            ISubscriberChannelRepository subscriberChannelRepository,
            TemplateEngine templateEngine,
            ILogger<ProcessEventHandler> logger
        )
        {
            _subscriptionRepository = subscriptionRepository;
            _subscriberRepository = subscriberRepository;
            _senders = senders;
            _templateRepository = templateRepository;
            _subscriberChannelRepository = subscriberChannelRepository;
            this.templateEngine = templateEngine;
            _logger = logger;
        }

        public async Task HandleAsync(ProcessEventCommand command)
        {
            _logger.LogInformation(
                "Processing event {EventType} for Subscriber: {SubscriberId}. Channel: {Channel}",
                command.EventType.Name,
                command.SubscriberId,
                command.Channel
            );

            var subscriber = await _subscriberRepository.GetByIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscriber not found");

            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscription not found");

            if (!subscription.CanHandle(command.EventType))
                throw new BusinessRuleException($"Subscription plan does not support event type: {command.EventType.Name}");

            if (!subscriber.ActiveChannels.Contains(command.Channel))
                throw new BusinessRuleException($"Subscriber is not enabled for channel: {command.Channel}");

            subscription.EnsureCanSendNotification();

            var template = await _templateRepository.GetTemplateAsync(command.EventType, command.Channel, command.SubscriberId)
                ?? throw new NotFoundException($"No template found for {command.EventType.Name} on channel {command.Channel.Name}");

            string subject = command.EventType.Name;
            string body = string.Join(", ", command.Parameters.Select(kv => $"{kv.Key}: {kv.Value}"));

            if (template != null)
            {
                var renderedSubject = templateEngine.Render(template.SubjectTemplate, command.Channel, command.Parameters);
                var renderedBody = templateEngine.Render(template.BodyTemplate, command.Channel, command.Parameters);

                if (!string.IsNullOrWhiteSpace(renderedSubject)) subject = renderedSubject;
                if (!string.IsNullOrWhiteSpace(renderedBody)) body = renderedBody;
            }

            var channelSetting = await _subscriberChannelRepository.GetSettingAsync(command.SubscriberId, command.Channel);

            var sender = _senders.FirstOrDefault(s => s.Channel.Equals(command.Channel));

            if (sender == null)
            {
                _logger.LogError("No implementation found for channel: {Channel}", command.Channel);
                throw new Exception($"System configuration error: No sender found for {command.Channel}");
            }

            _logger.LogInformation(
                "Sending notification via {Channel} to Recipient: {Recipient}",
                command.Channel,
                command.Recipient
            );

            await sender.SendAsync(command.SubscriberId, command.Recipient, command.EventType, subject, body, channelSetting?.Settings);

            subscription.IncreaseUsage();
            await _subscriptionRepository.SaveAsync(command.SubscriberId, subscription);

            _logger.LogInformation(
                "Notification sent successfully. Used: {Used}/{Limit}",
                subscription.UsedThisMonth,
                subscription.MonthlyLimit
            );
        }
    }
}