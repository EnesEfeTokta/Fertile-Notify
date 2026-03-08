using FertileNotify.Application.Contracts;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace FertileNotify.Infrastructure.BackgroundJobs
{
    public class NotificationConsumer : IConsumer<ProcessNotificationMessage>
    {
        private readonly ProcessEventHandler _handler;
        private readonly ILogger<NotificationConsumer> _logger;

        public NotificationConsumer(ProcessEventHandler handler, ILogger<NotificationConsumer> logger)
        {
            _handler = handler;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ProcessNotificationMessage> context)
        {
            var msg = context.Message;

            var command = new ProcessEventCommand
            {
                SubscriberId = msg.SubscriberId,
                Channel = NotificationChannel.From(msg.Channel),
                Recipient = msg.Recipient,
                EventType = EventType.From(msg.EventType),
                Parameters = msg.Parameters
            };

            await _handler.HandleAsync(command);
            _logger.LogInformation("Processed notification for Subscriber: {SubscriberId}, Channel: {Channel}, Event: {EventType}",
                command.SubscriberId, command.Channel.Name, command.EventType.Name);
        }
    }
}
