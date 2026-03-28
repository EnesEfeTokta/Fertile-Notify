using FertileNotify.Domain.Events;
using FluentAssertions;

namespace FertileNotify.Tests
{
    public class EventTypeTests
    {
        [Fact]
        public void From_WithValidName_ShouldReturnEventType()
        {
            // Arrange
            var name = "LoginAlert";

            // Act
            var result = EventType.From(name);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(name);
            result.Should().Be(EventType.LoginAlert);
        }

        [Fact]
        public void From_WithInvalidName_ShouldThrowArgumentException()
        {
            // Arrange
            var invalidName = "InvalidEvent";

            // Act
            Action act = () => EventType.From(invalidName);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage($"Unknown event type: {invalidName}");
        }

        [Theory]
        [InlineData("SubscriberRegistered")]
        [InlineData("PasswordReset")]
        [InlineData("EmailVerified")]
        [InlineData("OrderCreated")]
        [InlineData("Campaign")]
        public void From_WithAllValidNames_ShouldReturnCorrectEventType(string name)
        {
            // Act
            var result = EventType.From(name);

            // Assert
            result.Name.Should().Be(name);
        }
    }
}
