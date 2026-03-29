using FertileNotify.Application.Contracts;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.BackgroundJobs
{
    public class NotificationConsumer : IConsumer<ProcessNotificationMessage>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationConsumer> _logger;

        public NotificationConsumer(IMediator mediator, ILogger<NotificationConsumer> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProcessNotificationMessage> context)
        {
            await _mediator.Send(context.Message);
            _logger.LogInformation("Processed notification for Subscriber: {SubscriberId}, Channel: {Channel}, Event: {EventType}",
                context.Message.SubscriberId, context.Message.Channel, context.Message.EventType);
        }
    }
}
