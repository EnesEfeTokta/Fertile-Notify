namespace FertileNotify.Application.UseCases.SystemNotification
{
    public class ListSystemNotificationsHandler : IQueryHandler<ListSystemNotificationsQuery, List<SystemNotificationDto>>
    {
        private readonly ISystemNotificationRepository _notificationRepository;

        public ListSystemNotificationsHandler(ISystemNotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<List<SystemNotificationDto>> Handle(ListSystemNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _notificationRepository.GetBySubscriberIdAsync(request.SubscriberId, request.IsRead);
            return notifications.Select(SystemNotificationDto.FromNotification).ToList();
        }
    }

    public class MarkSystemNotificationAsReadHandler : IRequestHandler<MarkSystemNotificationAsReadCommand, Unit>
    {
        private readonly ISystemNotificationRepository _notificationRepository;

        public MarkSystemNotificationAsReadHandler(ISystemNotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public async Task<Unit> Handle(MarkSystemNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _notificationRepository.GetByIdAsync(request.NotificationId)
                ?? throw new NotFoundException("System notification not found.");

            if (notification.SubscriberId != request.SubscriberId)
                throw new NotFoundException("System notification not found.");

            await _notificationRepository.MarkAsReadAsync(request.NotificationId);
            return Unit.Value;
        }
    }
}