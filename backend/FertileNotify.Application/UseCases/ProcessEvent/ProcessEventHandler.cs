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
        private readonly TemplateEngine templateEngine;

        private readonly ILogger<ProcessEventHandler> _logger;

        public ProcessEventHandler(
            ISubscriptionRepository subscriptionRepository,
            ISubscriberRepository subscriberRepository,
            IEnumerable<INotificationSender> senders,
            ITemplateRepository templateRepository,
            TemplateEngine templateEngine,
            ILogger<ProcessEventHandler> logger
        )
        {
            _subscriptionRepository = subscriptionRepository;
            _subscriberRepository = subscriberRepository;
            _senders = senders;
            _templateRepository = templateRepository;
            this.templateEngine = templateEngine;
            _logger = logger;
        }

        public async Task HandleAsync(ProcessEventCommand command)
        {
            _logger.LogInformation(
                "Processing event {EventType} for Subscriber: {SubscriberId} with parameters: {Parameters}", 
                command.EventType.Name, 
                command.SubscriberId, 
                string.Join(", ", command.Parameters.Select(kv => $"{kv.Key}={kv.Value}"))
            );

            var subscriber = 
                await _subscriberRepository.GetByIdAsync(command.SubscriberId) 
                ?? throw new NotFoundException("Subscriber not found");

            var subscription =
                await _subscriptionRepository.GetBySubscriberIdAsync(command.SubscriberId)
                ?? throw new NotFoundException("Subscription not found");

            if (!subscription.CanHandle(command.EventType))
            {
                _logger.LogWarning(
                    "Subscription plan does not support event {EventType} for Subscriber: {SubscriberId}", 
                    command.EventType.Name, 
                    command.SubscriberId
                );
                throw new Exception("Subscription plan does not support this event type");
            }

            subscription.EnsureCanSendNotification();

            var template = 
                await _templateRepository.GetByEventTypeAsync(command.EventType) 
                ?? throw new Exception("Notification template not found");

            string subject = command.EventType.Name;
            string body = "No template available." ?? string.Empty;

            if (template != null)
            {
                subject = templateEngine.Render(template.SubjectTemplate, command.Parameters);
                body = templateEngine.Render(template.BodyTemplate, command.Parameters);
            }
            else
            {
                body = string.Join(Environment.NewLine, command.Parameters.Select(kv => $"{kv.Key}: {kv.Value}"));
            }

            bool handled = false;
            foreach (var channel in subscriber.ActiveChannels)
            {
                var sender = _senders.FirstOrDefault(s => s.Channel.Equals(channel));
                if (sender == null) continue;
                _logger.LogInformation(
                    "Sending notification via {Channel} to Subscriber: {SubscriberId}", 
                    channel, 
                    command.SubscriberId
                );
                await sender.SendAsync(command.Recipient, subject, body);
                handled = true;
            }

            if (handled)
            {
                subscription.IncreaseUsage();
                await _subscriptionRepository.SaveAsync(command.SubscriberId, subscription);
                _logger.LogInformation(
                    "Notification sent successfully for event {EventType} to Subscriber: {SubscriberId} - Used: {Used}", 
                    command.EventType.Name, 
                    command.SubscriberId,
                    subscription.UsedThisMonth
                );
            }
        }
    }
}