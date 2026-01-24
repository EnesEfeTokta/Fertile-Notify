using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using Moq;

namespace FertileNotify.Tests
{
    public class ProcessEventHandlerTests
    {
        private readonly Mock<ISubscriptionRepository> _mockSubRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ITemplateRepository> _mockTemplateRepo;
        private readonly Mock<INotificationSender> _mockEmailSender;
        private readonly TemplateEngine _templateEngine;

        private readonly ProcessEventHandler _handler;

        public ProcessEventHandlerTests()
        {
            _mockSubRepo = new Mock<ISubscriptionRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockTemplateRepo = new Mock<ITemplateRepository>();
            _mockEmailSender = new Mock<INotificationSender>();

            _mockEmailSender.Setup(x => x.Channel).Returns(NotificationChannel.Email);
            var senders = new List<INotificationSender> { _mockEmailSender.Object };

            _templateEngine = new TemplateEngine();

            _handler = new ProcessEventHandler(
                _mockSubRepo.Object,
                _mockUserRepo.Object,
                senders,
                _mockTemplateRepo.Object,
                _templateEngine
            );
        }

        [Fact]
        public async Task HandleAsync_Should_Send_Notification_When_Rules_Are_Valid()
        {
            var userId = Guid.NewGuid();
            var command = new ProcessEventCommand
            {
                UserId = userId,
                EventType = EventType.UserRegistered,
                Parameters = new Dictionary<string, string> { { "Name", "TestUser" } }
            };

            var user = new User(EmailAddress.Create("test@test.com"), new PhoneNumber("0"));
            _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

            var subscription = Subscription.Create(userId, SubscriptionPlan.Free);
            _mockSubRepo.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(subscription);

            var template = NotificationTemplate.Create(EventType.UserRegistered, "Subject {Name}", "Body");
            _mockTemplateRepo.Setup(x => x.GetByEventTypeAsync(EventType.UserRegistered)).ReturnsAsync(template);

            _mockEmailSender
                .Setup(x => x.SendAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            await _handler.HandleAsync(command);

            _mockEmailSender.Verify(
                x => x.SendAsync(
                    It.IsAny<User>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()
                ),
                Times.Once,
                "An email must be sent.");

            _mockSubRepo.Verify(
                x => x.SaveAsync(userId, It.IsAny<Subscription>()),
                Times.Once,
                "The subscription needs to be updated and saved.");
        }

        [Fact]
        public async Task HandleAsync_Should_Not_Send_When_Subscription_Expired()
        {
            var userId = Guid.NewGuid();
            var command = new ProcessEventCommand { UserId = userId, EventType = EventType.UserRegistered };

            var user = new User(EmailAddress.Create("test@test.com"), new PhoneNumber("0-0-0-0-"));
            _mockUserRepo.Setup(x => x.GetByIdAsync(userId)).ReturnsAsync(user);

            var subscription = Subscription.Create(userId, SubscriptionPlan.Free);

            var invalidCommand = new ProcessEventCommand { UserId = userId, EventType = EventType.OrderCreated };

            _mockSubRepo.Setup(x => x.GetByUserIdAsync(userId)).ReturnsAsync(subscription);

            await _handler.HandleAsync(invalidCommand);

            _mockEmailSender.Verify(
                x => x.SendAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>()),
                Times.Never);
        }
    }
}