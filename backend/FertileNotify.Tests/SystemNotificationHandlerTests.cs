using FertileNotify.Application.UseCases.SystemNotification;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace FertileNotify.Tests
{
    public class SystemNotificationHandlerTests
    {
        private readonly Mock<ISystemNotificationRepository> _notificationRepository;
        private readonly ListSystemNotificationsHandler _listHandler;
        private readonly MarkSystemNotificationAsReadHandler _markAsReadHandler;
        private readonly Guid _subscriberId;

        public SystemNotificationHandlerTests()
        {
            _notificationRepository = new Mock<ISystemNotificationRepository>();
            _listHandler = new ListSystemNotificationsHandler(_notificationRepository.Object);
            _markAsReadHandler = new MarkSystemNotificationAsReadHandler(_notificationRepository.Object);
            _subscriberId = Guid.NewGuid();
        }

        [Fact]
        public async Task ListNotifications_Should_Return_Filtered_Items()
        {
            var unreadNotification = new SystemNotification(_subscriberId, "Title 1", "Message 1");

            _notificationRepository
                .Setup(r => r.GetBySubscriberIdAsync(_subscriberId, false))
                .ReturnsAsync(new List<SystemNotification> { unreadNotification });

            var result = await _listHandler.Handle(new ListSystemNotificationsQuery
            {
                SubscriberId = _subscriberId,
                IsRead = false
            }, CancellationToken.None);

            result.Should().HaveCount(1);
            result[0].Title.Should().Be("Title 1");
            result[0].IsRead.Should().BeFalse();
        }

        [Fact]
        public async Task MarkAsRead_Should_Mark_Owned_Notification()
        {
            var notification = new SystemNotification(_subscriberId, "Title", "Message");
            var notificationId = notification.Id;

            _notificationRepository
                .Setup(r => r.GetByIdAsync(notificationId))
                .ReturnsAsync(notification);

            _notificationRepository
                .Setup(r => r.MarkAsReadAsync(notificationId))
                .Returns(Task.CompletedTask);

            await _markAsReadHandler.Handle(new MarkSystemNotificationAsReadCommand
            {
                SubscriberId = _subscriberId,
                NotificationId = notificationId
            }, CancellationToken.None);

            _notificationRepository.Verify(r => r.MarkAsReadAsync(notificationId), Times.Once);
        }

        [Fact]
        public async Task MarkAsRead_Should_Reject_Notifications_Belonging_To_Another_Subscriber()
        {
            var notification = new SystemNotification(Guid.NewGuid(), "Title", "Message");
            var notificationId = notification.Id;

            _notificationRepository
                .Setup(r => r.GetByIdAsync(notificationId))
                .ReturnsAsync(notification);

            var action = () => _markAsReadHandler.Handle(new MarkSystemNotificationAsReadCommand
            {
                SubscriberId = _subscriberId,
                NotificationId = notificationId
            }, CancellationToken.None);

            await action.Should().ThrowAsync<NotFoundException>();
        }
    }
}