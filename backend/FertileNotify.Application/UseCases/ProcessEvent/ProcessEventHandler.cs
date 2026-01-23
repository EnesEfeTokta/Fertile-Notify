using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;

namespace FertileNotify.Application.UseCases.ProcessEvent
{
    public class ProcessEventHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEnumerable<INotificationSender> _senders;
        private readonly ITemplateRepository _templateRepository;
        private readonly TemplateEngine templateEngine;

        public ProcessEventHandler(
            ISubscriptionRepository subscriptionRepository,
            IUserRepository userRepository,
            IEnumerable<INotificationSender> senders,
            ITemplateRepository templateRepository,
            TemplateEngine templateEngine
        )
        {
            _subscriptionRepository = subscriptionRepository;
            _userRepository = userRepository;
            _senders = senders;
            _templateRepository = templateRepository;
            this.templateEngine = templateEngine;
        }

        public async Task HandleAsync(ProcessEventCommand command)
        {
            var user = 
                await _userRepository.GetByIdAsync(command.UserId) 
                ?? throw new Exception("User not found");

            var subscription =
                await _subscriptionRepository.GetByUserIdAsync(command.UserId)
                ?? throw new Exception("Subscription not found");

            if (!subscription.CanHandle(command.EventType)) return;

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
                await sender.SendAsync(user, subject, body);
                handled = true;
            }

            if (!handled)
            {
                subscription.IncreaseUsage();
                await _subscriptionRepository.SaveAsync(command.UserId, subscription);
            }
        }
    }
}