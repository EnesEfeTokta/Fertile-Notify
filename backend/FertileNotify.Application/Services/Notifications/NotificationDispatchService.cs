namespace FertileNotify.Application.Services.Notifications
{
    public class NotificationDispatchService : INotificationDispatchService
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<NotificationDispatchService> _logger;

        public NotificationDispatchService(
            IPublishEndpoint publishEndpoint,
            ILogger<NotificationDispatchService> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task QueueAsync(
            ProcessNotificationMessage message,
            string source,
            CancellationToken cancellationToken = default)
        {
            var eventType = EventType.From(message.EventType);
            byte priority = eventType.GetPriority();

            await _publishEndpoint.Publish(message, context =>
            {
                context.Headers.Set("notification-priority", priority);
                context.Headers.Set("notification-source", source);
            }, cancellationToken);

            _logger.LogInformation(
                "[DISPATCH] Queued notification ({Source}) for {Recipient} on {Channel} with event {EventType}.",
                source,
                message.Recipient,
                message.Channel,
                message.EventType);
        }
    }
}
