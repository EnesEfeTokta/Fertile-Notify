using FertileNotify.Application.Contracts;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.Infrastructure.BackgroundJobs.Notifications
{
    public sealed class NotificationLogQueueItem
    {
        public DateTime QueuedAtUtc { get; init; } = DateTime.UtcNow;
        public Guid SubscriberId { get; init; }
        public string Recipient { get; init; } = string.Empty;
        public string Channel { get; init; } = string.Empty;
        public string EventTypeName { get; init; } = string.Empty;
        public string Subject { get; init; } = "[N/A]";
        public string Body { get; init; } = "[N/A]";
        public DeliveryStatus Status { get; init; }
        public string? ErrorMessage { get; init; }

        public static NotificationLogQueueItem Success(ProcessNotificationMessage message, NotificationContent content)
        {
            return new NotificationLogQueueItem
            {
                SubscriberId = message.SubscriberId,
                Recipient = message.Recipient,
                Channel = message.Channel,
                EventTypeName = message.EventType,
                Subject = content.Subject,
                Body = content.Body,
                Status = DeliveryStatus.Success
            };
        }

        public static NotificationLogQueueItem Failure(ProcessNotificationMessage message, NotificationContent? content, string errorReason)
        {
            return new NotificationLogQueueItem
            {
                SubscriberId = message.SubscriberId,
                Recipient = message.Recipient,
                Channel = message.Channel,
                EventTypeName = message.EventType,
                Subject = content?.Subject ?? "[N/A]",
                Body = content?.Body ?? "[N/A]",
                Status = DeliveryStatus.Failed,
                ErrorMessage = errorReason
            };
        }

        public static NotificationLogQueueItem Rejected(ProcessNotificationMessage message, NotificationContent? content, string errorReason)
        {
            return new NotificationLogQueueItem
            {
                SubscriberId = message.SubscriberId,
                Recipient = message.Recipient,
                Channel = message.Channel,
                EventTypeName = message.EventType,
                Subject = content?.Subject ?? "[N/A]",
                Body = content?.Body ?? "[N/A]",
                Status = DeliveryStatus.Rejected,
                ErrorMessage = errorReason
            };
        }

        public NotificationLog ToDomain()
        {
            var channel = NotificationChannel.From(Channel);
            var eventType = EventType.From(EventTypeName);
            var content = NotificationContent.Create(Subject, Body);

            var log = new NotificationLog(SubscriberId, Recipient, channel, eventType, content);
            switch (Status)
            {
                case DeliveryStatus.Success:
                    log.SetResult(true);
                    break;
                case DeliveryStatus.Rejected:
                    log.SetResult(false, ErrorMessage, isRejected: true);
                    break;
                default:
                    log.SetResult(false, ErrorMessage);
                    break;
            }

            return log;
        }
    }
}