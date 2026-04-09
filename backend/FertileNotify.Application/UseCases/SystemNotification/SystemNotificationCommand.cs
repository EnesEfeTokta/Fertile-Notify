namespace FertileNotify.Application.UseCases.SystemNotification
{
    public class ListSystemNotificationsQuery : IQuery<List<SystemNotificationDto>>
    {
        public Guid SubscriberId { get; set; }
        public bool? IsRead { get; set; }
    }

    public class MarkSystemNotificationAsReadCommand : IRequest<Unit>
    {
        public Guid SubscriberId { get; set; }
        public Guid NotificationId { get; set; }
    }
}
