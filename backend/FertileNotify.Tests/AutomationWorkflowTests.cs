using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using FluentAssertions;

namespace FertileNotify.Tests
{
    public class AutomationWorkflowTests
    {
        private readonly Guid _subscriberId = Guid.NewGuid();

        [Fact]
        public void Constructor_Should_Create_Workflow_With_Correct_Values()
        {
            // Arrange
            var subscriberId = Guid.NewGuid();
            var name = "Test Workflow";
            var description = "Test Description";
            var content = NotificationContent.Create("Subject", "Body");
            var channel = NotificationChannel.Email;
            var eventTrigger = "user_signup";
            var cronExpression = "";
            var recipients = new List<string> { "user@example.com" };

            // Act
            var workflow = new AutomationWorkflow(
                subscriberId, name, description, content, EventType.TestForDevelop, channel, 
                eventTrigger, cronExpression, recipients);

            // Assert
            workflow.Id.Should().NotBe(Guid.Empty);
            workflow.SubscriberId.Should().Be(subscriberId);
            workflow.Name.Should().Be(name);
            workflow.Description.Should().Be(description);
            workflow.Channel.Should().Be(channel);
            workflow.EventTrigger.Should().Be(eventTrigger);
            workflow.CronExpression.Should().Be(cronExpression);
            workflow.Recipients.Should().Equal(recipients);
            workflow.IsActive.Should().BeTrue();
            workflow.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Two_Workflows_Should_Have_Different_Ids()
        {
            // Arrange
            var content = NotificationContent.Create("Subject", "Body");

            // Act
            var workflow1 = new AutomationWorkflow(
                _subscriberId, "W1", "D1", content, EventType.TestForDevelop, NotificationChannel.Email, 
                "event1", "", new List<string> { "u1@example.com" });

            var workflow2 = new AutomationWorkflow(
                _subscriberId, "W2", "D2", content, EventType.TestForDevelop, NotificationChannel.SMS, 
                "event2", "", new List<string> { "+1234567890" });

            // Assert
            workflow1.Id.Should().NotBe(workflow2.Id);
        }

        [Fact]
        public void Activate_Should_Set_IsActive_To_True()
        {
            // Arrange
            var workflow = new AutomationWorkflow(
                _subscriberId, "Test", "Test", NotificationContent.Create("S", "B"), 
                EventType.TestForDevelop, NotificationChannel.Email, "event", "", new List<string> { "user@example.com" });
            workflow.Deactivate();

            // Act
            workflow.Activate();

            // Assert
            workflow.IsActive.Should().BeTrue();
        }

        [Fact]
        public void Deactivate_Should_Set_IsActive_To_False()
        {
            // Arrange
            var workflow = new AutomationWorkflow(
                _subscriberId, "Test", "Test", NotificationContent.Create("S", "B"), 
                EventType.TestForDevelop, NotificationChannel.Email, "event", "", new List<string> { "user@example.com" });

            // Act
            workflow.Deactivate();

            // Assert
            workflow.IsActive.Should().BeFalse();
        }

        [Fact]
        public void UpdateDetails_Should_Update_Name_And_Description()
        {
            // Arrange
            var workflow = new AutomationWorkflow(
                _subscriberId, "Old Name", "Old Description", NotificationContent.Create("S", "B"), 
                EventType.TestForDevelop, NotificationChannel.Email, "event", "", new List<string> { "user@example.com" });

            // Act
            workflow.UpdateDetails("New Name", "New Description");

            // Assert
            workflow.Name.Should().Be("New Name");
            workflow.Description.Should().Be("New Description");
        }

        [Fact]
        public void UpdateContent_Should_Replace_Content()
        {
            // Arrange
            var oldContent = NotificationContent.Create("Old Subject", "Old Body");
            var newContent = NotificationContent.Create("New Subject", "New Body");

            var workflow = new AutomationWorkflow(
                _subscriberId, "Test", "Test", oldContent, EventType.TestForDevelop, NotificationChannel.Email, 
                "event", "", new List<string> { "user@example.com" });

            // Act
            workflow.UpdateContent(newContent);

            // Assert
            workflow.Content.Should().Be(newContent);
        }

        [Fact]
        public void UpdateChannel_Should_Replace_Channel()
        {
            // Arrange
            var workflow = new AutomationWorkflow(
                _subscriberId, "Test", "Test", NotificationContent.Create("S", "B"), 
                EventType.TestForDevelop, NotificationChannel.Email, "event", "", new List<string> { "user@example.com" });

            // Act
            workflow.UpdateChannel(NotificationChannel.SMS);

            // Assert
            workflow.Channel.Should().Be(NotificationChannel.SMS);
        }

        [Fact]
        public void UpdateRecipients_Should_Replace_Recipients()
        {
            // Arrange
            var oldRecipients = new List<string> { "user1@example.com" };
            var newRecipients = new List<string> { "user2@example.com", "user3@example.com" };

            var workflow = new AutomationWorkflow(
                _subscriberId, "Test", "Test", NotificationContent.Create("S", "B"), 
                EventType.TestForDevelop, NotificationChannel.Email, "event", "", oldRecipients);

            // Act
            workflow.UpdateRecipients(newRecipients);

            // Assert
            workflow.Recipients.Should().Equal(newRecipients);
        }

        [Fact]
        public void UpdateSchedule_Should_Update_EventTrigger_And_CronExpression()
        {
            // Arrange
            var workflow = new AutomationWorkflow(
                _subscriberId, "Test", "Test", NotificationContent.Create("S", "B"), 
                EventType.TestForDevelop, NotificationChannel.Email, "old_event", "0 0 * * *", new List<string> { "user@example.com" });

            // Act
            workflow.UpdateSchedule("new_event", "0 12 * * *");

            // Assert
            workflow.EventTrigger.Should().Be("new_event");
            workflow.CronExpression.Should().Be("0 12 * * *");
        }

        [Fact]
        public void Multiple_Updates_Should_Preserve_Other_Properties()
        {
            // Arrange
            var workflow = new AutomationWorkflow(
                _subscriberId, "Original", "Description", NotificationContent.Create("Subject", "Body"), 
                EventType.TestForDevelop, NotificationChannel.Email, "event", "", new List<string> { "user@example.com" });
            var originalId = workflow.Id;
            var originalSubscriberId = workflow.SubscriberId;
            var originalCreatedAt = workflow.CreatedAt;

            // Act
            workflow.UpdateDetails("Updated", "Updated Description");
            workflow.Activate();
            workflow.UpdateChannel(NotificationChannel.SMS);

            // Assert
            workflow.Id.Should().Be(originalId);
            workflow.SubscriberId.Should().Be(originalSubscriberId);
            workflow.CreatedAt.Should().Be(originalCreatedAt);
            workflow.Name.Should().Be("Updated");
            workflow.Channel.Should().Be(NotificationChannel.SMS);
            workflow.IsActive.Should().BeTrue();
        }
    }
}
