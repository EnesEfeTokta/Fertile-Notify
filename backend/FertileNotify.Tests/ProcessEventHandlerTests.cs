using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;

namespace FertileNotify.Tests
{
    public class ProcessEventHandlerTests
    {
        private readonly Mock<ISubscriptionRepository> _mockSubRepo;
        private readonly Mock<ISubscriberRepository> _mockSubscriberRepo;
        private readonly Mock<ITemplateRepository> _mockTemplateRepo;
        private readonly Mock<INotificationSender> _mockEmailSender;
        private readonly Mock<ILogger<ProcessEventHandler>> _mockLogger;
        private readonly TemplateEngine _templateEngine;

        private readonly ProcessEventHandler _handler;

        public ProcessEventHandlerTests()
        {
            _mockSubRepo = new Mock<ISubscriptionRepository>();
            _mockSubscriberRepo = new Mock<ISubscriberRepository>();
            _mockTemplateRepo = new Mock<ITemplateRepository>();
            _mockEmailSender = new Mock<INotificationSender>();
            _mockLogger = new Mock<ILogger<ProcessEventHandler>>();

            _mockEmailSender.Setup(x => x.Channel).Returns(NotificationChannel.Email);
            var senders = new List<INotificationSender> { _mockEmailSender.Object };

            _templateEngine = new TemplateEngine();

            _handler = new ProcessEventHandler(
                _mockSubRepo.Object,
                _mockSubscriberRepo.Object,
                senders,
                _mockTemplateRepo.Object,
                _templateEngine,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task HandleAsync_Should_Send_Notification_When_Rules_Are_Valid()
        {
            var subscriberId = Guid.NewGuid();
            var command = new ProcessEventCommand
            {
                SubscriberId = subscriberId,
                Channel = NotificationChannel.Email,
                Recipient = "customer@example.com",
                EventType = EventType.SubscriberRegistered,
                Parameters = new Dictionary<string, string> { { "Name", "TestUser" } }
            };

            var subscriber = new Subscriber(
                CompanyName.Create("Company Test"),
                EmailAddress.Create("company@test.com"), 
                null);
            _mockSubscriberRepo.Setup(x => x.GetByIdAsync(subscriberId)).ReturnsAsync(subscriber);

            var subscription = Subscription.Create(subscriberId, SubscriptionPlan.Free);
            _mockSubRepo.Setup(x => x.GetBySubscriberIdAsync(subscriberId)).ReturnsAsync(subscription);

            var template = NotificationTemplate.Create(EventType.SubscriberRegistered, "Subject {Name}", "Body");
            _mockTemplateRepo.Setup(x => x.GetByEventTypeAsync(EventType.SubscriberRegistered)).ReturnsAsync(template);

            _mockEmailSender
                .Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await _handler.HandleAsync(command);

            _mockEmailSender.Verify(
                x => x.SendAsync(
                    "customer@example.com",
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
                Times.Once,
                "An email must be sent to the recipient.");

            _mockSubRepo.Verify(
                x => x.SaveAsync(subscriberId, It.IsAny<Subscription>()),
                Times.Once,
                "The subscription usage needs to be updated.");
        }

        [Fact]
        public async Task HandleAsync_Should_Not_Send_When_Plan_Does_Not_Support_Event()
        {
            var subscriberId = Guid.NewGuid();

            var command = new ProcessEventCommand
            {
                SubscriberId = subscriberId,
                Channel = NotificationChannel.Email,
                Recipient = "customer@example.com",
                EventType = EventType.OrderCreated
            };

            var subscriber = new Subscriber(
                CompanyName.Create("Company Test"),
                EmailAddress.Create("company@test.com"),
                null);
            _mockSubscriberRepo.Setup(x => x.GetByIdAsync(subscriberId)).ReturnsAsync(subscriber);

            var subscription = Subscription.Create(subscriberId, SubscriptionPlan.Free);
            _mockSubRepo.Setup(x => x.GetBySubscriberIdAsync(subscriberId)).ReturnsAsync(subscription);

            await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.HandleAsync(command));

            _mockEmailSender.Verify(
                x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}