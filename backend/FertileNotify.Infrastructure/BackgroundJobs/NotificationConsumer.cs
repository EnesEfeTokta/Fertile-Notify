using FertileNotify.Application.Contracts;
using FertileNotify.Application.UseCases.SendNotification;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.BackgroundJobs
{
    public class NotificationConsumer : IConsumer<ProcessNotificationMessage>
    {
        private readonly SendNotificationHandler _handler;
        private readonly ILogger<NotificationConsumer> _logger;

        public NotificationConsumer(SendNotificationHandler handler, ILogger<NotificationConsumer> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProcessNotificationMessage> context)
        {
            await _handler.ProcessNotificationAsync(context.Message);
            _logger.LogInformation("Processed notification for Subscriber: {SubscriberId}, Channel: {Channel}, Event: {EventType}",
                context.Message.SubscriberId, context.Message.Channel, context.Message.EventType);
        }
    }
}
