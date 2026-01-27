using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;
using FertileNotify.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Application.UseCases.ProcessEvent
{
    public class ProcessEventHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEnumerable<INotificationSender> _senders;
        private readonly ITemplateRepository _templateRepository;
        private readonly TemplateEngine templateEngine;

        private readonly ILogger<ProcessEventHandler> _logger;

        public ProcessEventHandler(
            ISubscriptionRepository subscriptionRepository,
            IUserRepository userRepository,
            IEnumerable<INotificationSender> senders,
            ITemplateRepository templateRepository,
            TemplateEngine templateEngine,
            ILogger<ProcessEventHandler> logger
        )
        {
            _subscriptionRepository = subscriptionRepository;
            _userRepository = userRepository;
            _senders = senders;
            _templateRepository = templateRepository;
            this.templateEngine = templateEngine;
            _logger = logger;
        }

        public async Task HandleAsync(ProcessEventCommand command)
        {
            _logger.LogInformation(
                "Processing event {EventType} for User: {UserId} with parameters: {Parameters}", 
                command.EventType.Name, 
                command.UserId, 
                string.Join(", ", command.Parameters.Select(kv => $"{kv.Key}={kv.Value}"))
            );

            var user = 
                await _userRepository.GetByIdAsync(command.UserId) 
                ?? throw new NotFoundException("User not found");

            var subscription =
                await _subscriptionRepository.GetByUserIdAsync(command.UserId)
                ?? throw new NotFoundException("Subscription not found");

            if (!subscription.CanHandle(command.EventType))
            {
                _logger.LogWarning(
                    "Subscription plan does not support event {EventType} for User: {UserId}", 
                    command.EventType.Name, 
                    command.UserId
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
            foreach (var channel in user.ActiveChannels)
            {
                var sender = _senders.FirstOrDefault(s => s.Channel.Equals(channel));
                if (sender == null) continue;
                _logger.LogInformation(
                    "Sending notification via {Channel} to User: {UserId}", 
                    channel, 
                    command.UserId
                );
                await sender.SendAsync(user, subject, body);
                handled = true;
            }

            if (handled)
            {
                subscription.IncreaseUsage();
                await _subscriptionRepository.SaveAsync(command.UserId, subscription);
                _logger.LogInformation(
                    "Notification sent successfully for event {EventType} to User: {UserId} - Used: {Used}", 
                    command.EventType.Name, 
                    command.UserId,
                    subscription.UsedThisMonth
                );
            }
        }
    }
}