using System;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace FertileNotify.Tests
{
    public class NotificationTemplateTests
    {
        [Fact]
        public void CreateGlobal_Should_InitializePropertiesCorrectly()
        {
            // Arrange
            var name = "Welcome Email";
            var description = "Standard welcome message for new subscribers";
            var eventType = EventType.SubscriberRegistered;
            var channel = NotificationChannel.Email;
            var subject = "Welcome to our service!";
            var body = "Hi {{name}}, thanks for joining!";

            // Act
            var template = FertileNotify.Domain.Entities.NotificationTemplate.CreateGlobal(name, description, eventType, channel, subject, body);

            // Assert
            template.Id.Should().NotBeEmpty();
            template.SubscriberId.Should().BeNull();
            template.Name.Should().Be(name);
            template.Description.Should().Be(description);
            template.EventType.Should().Be(eventType);
            template.Channel.Should().Be(channel);
            template.Subject.Should().Be(subject);
            template.Body.Should().Be(body);
        }

        [Fact]
        public void CreateCustom_Should_InitializePropertiesCorrectly()
        {
            // Arrange
            var subscriberId = Guid.NewGuid();
            var name = "Custom Order Notification";
            var description = "A custom notification for a specific subscriber's orders";
            var eventType = EventType.OrderCreated;
            var channel = NotificationChannel.SMS;
            var subject = "Your Order is Confirmed";
            var body = "Your order #{{orderId}} is being processed.";

            // Act
            var template = FertileNotify.Domain.Entities.NotificationTemplate.CreateCustom(subscriberId, name, description, eventType, channel, subject, body);

            // Assert
            template.Id.Should().NotBeEmpty();
            template.SubscriberId.Should().Be(subscriberId);
            template.Name.Should().Be(name);
            template.Description.Should().Be(description);
            template.EventType.Should().Be(eventType);
            template.Channel.Should().Be(channel);
            template.Subject.Should().Be(subject);
            template.Body.Should().Be(body);
        }

        [Fact]
        public void Update_Should_ModifyOnlySpecificProperties()
        {
            // Arrange
            var template = FertileNotify.Domain.Entities.NotificationTemplate.CreateGlobal(
                "Initial Name",
                "Initial Description",
                EventType.TestForDevelop,
                NotificationChannel.Console,
                "Initial Subject",
                "Initial Body"
            );

            var initialId = template.Id;
            var initialEventType = template.EventType;
            var initialChannel = template.Channel;

            var updatedName = "Updated Name";
            var updatedDescription = "Updated Description";
            var updatedSubject = "Updated Subject";
            var updatedBody = "Updated Body";

            // Act
            template.Update(updatedName, updatedDescription, updatedSubject, updatedBody);

            // Assert
            template.Name.Should().Be(updatedName);
            template.Description.Should().Be(updatedDescription);
            template.Subject.Should().Be(updatedSubject);
            template.Body.Should().Be(updatedBody);

            // Properties that should NOT change
            template.Id.Should().Be(initialId);
            template.SubscriberId.Should().BeNull();
            template.EventType.Should().Be(initialEventType);
            template.Channel.Should().Be(initialChannel);
        }
    }
}
