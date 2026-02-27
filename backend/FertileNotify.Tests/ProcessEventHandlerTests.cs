using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Mjml.Net;
using Moq;

namespace FertileNotify.Tests
{
    public class ProcessEventHandlerTests
    {
        private readonly Mock<ISubscriptionRepository> _mockSubRepo;
        private readonly Mock<ISubscriberRepository> _mockSubscriberRepo;
        private readonly Mock<ITemplateRepository> _mockTemplateRepo;
        private readonly Mock<ISubscriberChannelRepository> _mockSubscriberChannelRepo;
        private readonly Mock<INotificationSender> _mockEmailSender;
        private readonly Mock<ILogger<ProcessEventHandler>> _mockLogger;
        private readonly Mock<IMjmlRenderer> _mockMjmlRenderer;
        private readonly TemplateEngine _templateEngine;

        private readonly ProcessEventHandler _handler;

        public ProcessEventHandlerTests()
        {
            _mockSubRepo = new Mock<ISubscriptionRepository>();
            _mockSubscriberRepo = new Mock<ISubscriberRepository>();
            _mockTemplateRepo = new Mock<ITemplateRepository>();
            _mockSubscriberChannelRepo = new Mock<ISubscriberChannelRepository>();
            _mockEmailSender = new Mock<INotificationSender>();
            _mockLogger = new Mock<ILogger<ProcessEventHandler>>();
            _mockMjmlRenderer = new Mock<IMjmlRenderer>();

            _mockEmailSender.Setup(x => x.Channel).Returns(NotificationChannel.Email);
            var senders = new List<INotificationSender> { _mockEmailSender.Object };

            _mockMjmlRenderer.Setup(x => x.Render(It.IsAny<string>(), It.IsAny<MjmlOptions>()))
                .Returns(new RenderResult("<html></html>", new ValidationErrors()));

            _templateEngine = new TemplateEngine(_mockMjmlRenderer.Object);

            _handler = new ProcessEventHandler(
                _mockSubRepo.Object,
                _mockSubscriberRepo.Object,
                senders,
                _mockTemplateRepo.Object,
                _mockSubscriberChannelRepo.Object,
                _templateEngine,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task HandleAsync_Should_Send_Notification_When_Rules_Are_Valid()
        {
            // ARRANGE
            var subscriberId = Guid.NewGuid();
            var command = new ProcessEventCommand
            {
                SubscriberId = subscriberId,
                Channel = NotificationChannel.Email,
                Recipient = "customer@example.com",
                EventType = EventType.SubscriberRegistered,
                Parameters = new Dictionary<string, string> { { "Name", "Enes" } }
            };

            var subscriber = new Subscriber(
                CompanyName.Create("Company Test"),
                Password.Create("StrongP@ssw0rd1!"),
                EmailAddress.Create("company@test.com"),
                null);
            _mockSubscriberRepo.Setup(x => x.GetByIdAsync(subscriberId)).ReturnsAsync(subscriber);

            var subscription = Subscription.Create(subscriberId, SubscriptionPlan.Free);
            _mockSubRepo.Setup(x => x.GetBySubscriberIdAsync(subscriberId)).ReturnsAsync(subscription);

            var template = NotificationTemplate.CreateGlobal(
                "Name",
                "Description",
                EventType.SubscriberRegistered,
                NotificationChannel.Email,
                "Subject",
                "Body");

            _mockTemplateRepo.Setup(x => x.GetTemplateAsync(EventType.SubscriberRegistered, NotificationChannel.Email, subscriberId))
                .ReturnsAsync(template);

            _mockSubscriberChannelRepo.Setup(x => x.GetSettingAsync(subscriberId, NotificationChannel.Email))
                .ReturnsAsync((SubscriberChannelSetting)null!);

            _mockEmailSender
                .Setup(x => x.SendAsync(subscriberId, "customer@example.com", EventType.SubscriberRegistered, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>()))
                .ReturnsAsync(true);

            // ACT
            await _handler.HandleAsync(command);

            // ASSERT
            _mockEmailSender.Verify(
                x => x.SendAsync(
                    subscriberId,
                    "customer@example.com",
                    EventType.SubscriberRegistered,
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<IReadOnlyDictionary<string, string>>()
                ),
                Times.Once);

            _mockSubRepo.Verify(
                x => x.SaveAsync(subscriberId, It.IsAny<Subscription>()),
                Times.Once);
        }

        //[Fact]
        //public async Task HandleAsync_Should_Not_Send_When_Plan_Does_Not_Support_Event()
        //{
        //    // ARRANGE
        //    var subscriberId = Guid.NewGuid();
        //    var command = new ProcessEventCommand
        //    {
        //        SubscriberId = subscriberId,
        //        Channel = NotificationChannel.Email,
        //        Recipient = "customer@example.com",
        //        EventType = EventType.OrderCreated, // Free paket bunu desteklemez
        //    };

        //    var subscriber = new Subscriber(
        //        CompanyName.Create("Company Test"),
        //        Password.Create("StrongP@ssw0rd1!"),
        //        EmailAddress.Create("company@test.com"),
        //        null);
        //    _mockSubscriberRepo.Setup(x => x.GetByIdAsync(subscriberId)).ReturnsAsync(subscriber);

        //    var subscription = Subscription.Create(subscriberId, SubscriptionPlan.Free);
        //    _mockSubRepo.Setup(x => x.GetBySubscriberIdAsync(subscriberId)).ReturnsAsync(subscription);

        //    // ACT & ASSERT
        //    await Assert.ThrowsAsync<BusinessRuleException>(() => _handler.HandleAsync(command));

        //    _mockEmailSender.Verify(
        //        x => x.SendAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<EventType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IReadOnlyDictionary<string, string>>()),
        //        Times.Never);
        //}
    }
}