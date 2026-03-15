using FertileNotify.Application.Contracts;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.SendNotification;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.ValueObjects;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace FertileNotify.Tests
{
    //public class SendNotificationHandlerTests
    //{
    //    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    //    private readonly Mock<IBlacklistRepository> _mockBlacklistRepository;
    //    private readonly Mock<ISubscriptionRepository> _mockSubscriptionRepository;
    //    private readonly Mock<ISubscriberRepository> _mockSubscriberRepository;
    //    private readonly Mock<ITemplateRepository> _mockTemplateRepository;
    //    private readonly Mock<ISubscriberChannelRepository> _mockSubscriberChannelRepository;
    //    private readonly Mock<IStatsRepository> _mockStatsRepository;
    //    private readonly Mock<ISecurityService> _mockSecurityService;
    //    private readonly Mock<ILogger<SendNotificationHandler>> _mockLogger;
    //    private readonly SendNotificationHandler _handler;

    //    public SendNotificationHandlerTests()
    //    {
    //        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
    //        _mockBlacklistRepository = new Mock<IBlacklistRepository>();
    //        _mockSubscriptionRepository = new Mock<ISubscriptionRepository>();
    //        _mockSubscriberRepository = new Mock<ISubscriberRepository>();
    //        _mockTemplateRepository = new Mock<ITemplateRepository>();
    //        _mockSubscriberChannelRepository = new Mock<ISubscriberChannelRepository>();
    //        _mockStatsRepository = new Mock<IStatsRepository>();
    //        _mockSecurityService = new Mock<ISecurityService>();
    //        _mockLogger = new Mock<ILogger<SendNotificationHandler>>();

    //        _handler = new SendNotificationHandler(
    //            _mockPublishEndpoint.Object,
    //            _mockBlacklistRepository.Object,
    //            _mockSubscriptionRepository.Object,
    //            _mockSubscriberRepository.Object,
    //            new List<INotificationSender>(),
    //            _mockTemplateRepository.Object,
    //            _mockSubscriberChannelRepository.Object,
    //            null!,
    //            _mockStatsRepository.Object,
    //            _mockSecurityService.Object,
    //            _mockLogger.Object
    //        );
    //    }

    //    [Fact]
    //    public async Task HandleAsync_Should_Publish_Messages_For_All_NonBlacklisted_Recipients()
    //    {
    //        // ARRANGE
    //        var subscriberId = Guid.NewGuid();
    //        var command = new SendNotificationCommand
    //        {
    //            SubscriberId = subscriberId,
    //            EventType = "LoginAlert",
    //            To = new List<ChannelRecipientCommandGroup>
    //            {
    //                new ChannelRecipientCommandGroup
    //                {
    //                    Channel = "Email",
    //                    Recipients = new List<string> { "user1@example.com", "user2@example.com" }
    //                }
    //            }
    //        };

    //        _mockBlacklistRepository
    //            .Setup(x => x.GetForRecipientsAsync(subscriberId, It.IsAny<List<string>>()))
    //            .ReturnsAsync(new List<ForbiddenRecipient>());

    //        // ACT
    //        var result = await _handler.HandleAsync(command);

    //        // ASSERT
    //        result.Should().Be(2);
    //        _mockPublishEndpoint.Verify(x => x.Publish<ProcessNotificationMessage>(
    //            It.IsAny<object>(),
    //            It.IsAny<Action<PublishContext<ProcessNotificationMessage>>>(),
    //            It.IsAny<CancellationToken>()), Times.Exactly(2));
    //    }

    //    [Fact]
    //    public async Task HandleAsync_Should_Skip_Blacklisted_Recipients()
    //    {
    //        // ARRANGE
    //        var subscriberId = Guid.NewGuid();
    //        var command = new SendNotificationCommand
    //        {
    //            SubscriberId = subscriberId,
    //            EventType = "LoginAlert",
    //            To = new List<ChannelRecipientCommandGroup>
    //            {
    //                new ChannelRecipientCommandGroup
    //                {
    //                    Channel = "Email",
    //                    Recipients = new List<string> { "blacklisted@example.com", "valid@example.com" }
    //                }
    //            }
    //        };

    //        var blacklistedItems = new List<ForbiddenRecipient>
    //        {
    //            new ForbiddenRecipient(subscriberId, "blacklisted@example.com", new List<NotificationChannel>())
    //        };

    //        _mockBlacklistRepository
    //            .Setup(x => x.GetForRecipientsAsync(subscriberId, It.IsAny<List<string>>()))
    //            .ReturnsAsync(blacklistedItems);

    //        // ACT
    //        var result = await _handler.HandleAsync(command);

    //        // ASSERT
    //        result.Should().Be(1);
    //        _mockPublishEndpoint.Verify(x => x.Publish<ProcessNotificationMessage>(
    //            It.IsAny<object>(),
    //            It.IsAny<Action<PublishContext<ProcessNotificationMessage>>>(),
    //            It.IsAny<CancellationToken>()), Times.Once);
    //    }
    //}
}
