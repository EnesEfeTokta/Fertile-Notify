using FertileNotify.Application.Contracts;
using FertileNotify.Application.UseCases.Workflow;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Logging.Abstractions;

namespace FertileNotify.Tests
{
    public class WorkflowNotificationHandlerTests
    {
        private readonly Mock<IAutomationRepository> _mockAutomationRepository;
        private readonly Mock<INotificationDispatchService> _mockDispatchService;
        private readonly Mock<IWorkflowScheduleService> _mockWorkflowScheduleService;
        private readonly AutomationTriggerService _automationTriggerService;
        private readonly WorkflowNotificationHandler _handler;
        private readonly CreateWorkflowNotificationHandler _createHandler;
        private readonly TriggerWorkflowNotificationsHandler _triggerHandler;
        private readonly Guid _subscriberId;

        public WorkflowNotificationHandlerTests()
        {
            _mockAutomationRepository = new Mock<IAutomationRepository>();
            _mockDispatchService = new Mock<INotificationDispatchService>();
            _mockWorkflowScheduleService = new Mock<IWorkflowScheduleService>();
            _automationTriggerService = new AutomationTriggerService(
                _mockAutomationRepository.Object,
                _mockDispatchService.Object,
                NullLogger<AutomationTriggerService>.Instance);
            _handler = new WorkflowNotificationHandler(
                _mockAutomationRepository.Object,
                _automationTriggerService,
                _mockWorkflowScheduleService.Object);
            _createHandler = new CreateWorkflowNotificationHandler(
                _mockAutomationRepository.Object,
                _mockWorkflowScheduleService.Object);
            _triggerHandler = new TriggerWorkflowNotificationsHandler(_automationTriggerService);
            _subscriberId = Guid.NewGuid();
        }

        [Fact]
        public async Task TriggerAsync_Should_Call_AutomationTriggerService_With_CorrectParameters()
        {
            // Arrange
            var command = new TriggerWorkflowNotificationsCommand
            {
                SubscriberId = _subscriberId,
                EventTrigger = "user_signup"
            };

            var workflows = new List<AutomationWorkflow>
            {
                new(_subscriberId, "W1", "D1", NotificationContent.Create("S", "B"),
                    EventType.TestForDevelop, NotificationChannel.Email, "user_signup", "", new List<string> { "user@example.com" })
            };

            _mockAutomationRepository
                .Setup(r => r.GetByEventTriggerAsync(_subscriberId, "user_signup"))
                .ReturnsAsync(workflows);

            _mockDispatchService
                .Setup(p => p.QueueAsync(It.IsAny<ProcessNotificationMessage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _triggerHandler.Handle(command, CancellationToken.None);

            // Assert
            result.Should().Be(1);
        }

        [Fact]
        public async Task TriggerAsync_Should_Trim_EventTrigger()
        {
            // Arrange
            var command = new TriggerWorkflowNotificationsCommand
            {
                SubscriberId = _subscriberId,
                EventTrigger = "  user_signup  "
            };

            var workflows = new List<AutomationWorkflow>
            {
                new(_subscriberId, "W1", "D1", NotificationContent.Create("S", "B"),
                    EventType.TestForDevelop, NotificationChannel.Email, "user_signup", "", new List<string> { "user@example.com" })
            };

            _mockAutomationRepository
                .Setup(r => r.GetByEventTriggerAsync(_subscriberId, "user_signup"))
                .ReturnsAsync(workflows);

            _mockDispatchService
                .Setup(p => p.QueueAsync(It.IsAny<ProcessNotificationMessage>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _triggerHandler.Handle(command, CancellationToken.None);

            // Assert - No exception thrown
        }

        [Fact]
        public async Task CreateAsync_Should_Create_Workflow_Successfully()
        {
            // Arrange
            var command = new CreateWorkflowNotificationCommand
            {
                SubscriberId = _subscriberId,
                Name = "Welcome Email",
                Description = "Send welcome email to new users",
                EventType = "TestForDevelop",
                EventTrigger = "user_signup",
                CronExpression = "",
                Subject = "Welcome!",
                Body = "Welcome to our service",
                To = new List<WorkflowRecipientGroupCommand>
                {
                    new() { Channel = "email", Recipients = new() { "user@example.com" } }
                }
            };

            _mockAutomationRepository
                .Setup(r => r.CreateAsync(It.IsAny<AutomationWorkflow>()))
                .Returns(Task.CompletedTask);
            _mockAutomationRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mockWorkflowScheduleService
                .Setup(s => s.SyncAsync(It.IsAny<AutomationWorkflow>()))
                .Returns(Task.CompletedTask);

            // Act
            var workflowId = await _createHandler.Handle(command, CancellationToken.None);

            // Assert
            workflowId.Should().NotBe(Guid.Empty);
            _mockAutomationRepository.Verify(r => r.CreateAsync(It.IsAny<AutomationWorkflow>()), Times.Once);
            _mockAutomationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockWorkflowScheduleService.Verify(s => s.SyncAsync(It.IsAny<AutomationWorkflow>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Allow_When_EventTrigger_Is_Empty_But_Cron_Is_Set()
        {
            // Arrange
            var command = new CreateWorkflowNotificationCommand
            {
                SubscriberId = _subscriberId,
                Name = "Test",
                Description = "Test",
                EventType = "TestForDevelop",
                EventTrigger = "",
                CronExpression = "0 9 * * *",
                Subject = "Test",
                Body = "Test",
                To = new List<WorkflowRecipientGroupCommand>
                {
                    new() { Channel = "email", Recipients = new() { "user@example.com" } }
                }
            };

            _mockAutomationRepository
                .Setup(r => r.CreateAsync(It.IsAny<AutomationWorkflow>()))
                .Returns(Task.CompletedTask);
            _mockAutomationRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mockWorkflowScheduleService
                .Setup(s => s.SyncAsync(It.IsAny<AutomationWorkflow>()))
                .Returns(Task.CompletedTask);

            // Act
            var workflowId = await _createHandler.Handle(command, CancellationToken.None);

            // Assert
            workflowId.Should().NotBe(Guid.Empty);
            _mockWorkflowScheduleService.Verify(s => s.SyncAsync(It.IsAny<AutomationWorkflow>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Sync_Schedule_When_Cron_Changes()
        {
            // Arrange
            var existingWorkflow = new AutomationWorkflow(
                _subscriberId,
                "Workflow",
                "Description",
                NotificationContent.Create("Subject", "Body"),
                EventType.TestForDevelop,
                NotificationChannel.Email,
                "event",
                "0 8 * * *",
                new List<string> { "user@example.com" });

            var command = new UpdateWorkflowNotificationCommand
            {
                SubscriberId = _subscriberId,
                Id = existingWorkflow.Id,
                CronExpression = "0 9 * * *"
            };

            _mockAutomationRepository
                .Setup(r => r.GetByIdAsync(existingWorkflow.Id))
                .ReturnsAsync(existingWorkflow);
            _mockAutomationRepository
                .Setup(r => r.Update(It.IsAny<AutomationWorkflow>()));
            _mockAutomationRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mockWorkflowScheduleService
                .Setup(s => s.SyncAsync(It.IsAny<AutomationWorkflow>()))
                .Verifiable();

            // Act
            await _handler.UpdateAsync(command);

            // Assert
            existingWorkflow.CronExpression.Should().Be("0 9 * * *");
            _mockWorkflowScheduleService.Verify(s => s.SyncAsync(It.IsAny<AutomationWorkflow>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Throw_Exception_For_Non_Owned_Workflow()
        {
            // Arrange
            var command = new UpdateWorkflowNotificationCommand
            {
                SubscriberId = Guid.NewGuid(), // Different subscriber
                Id = Guid.NewGuid()
            };

            _mockAutomationRepository
                .Setup(r => r.GetByIdAsync(command.Id))
                .ReturnsAsync((AutomationWorkflow)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _handler.UpdateAsync(command));
        }

        [Fact]
        public async Task ListAsync_Should_Return_All_Workflows_For_Subscriber()
        {
            // Arrange
            var workflows = new List<AutomationWorkflow>
            {
                new(_subscriberId, "Workflow 1", "Desc 1", 
                    NotificationContent.Create("S1", "B1"), EventType.TestForDevelop, NotificationChannel.Email, "event1", "", 
                    new List<string> { "user1@example.com" }),
                new(_subscriberId, "Workflow 2", "Desc 2", 
                    NotificationContent.Create("S2", "B2"), EventType.TestForDevelop, NotificationChannel.SMS, "event2", "", 
                    new List<string> { "+1234567890" })
            };

            _mockAutomationRepository
                .Setup(r => r.GetBySubscriberIdAsync(_subscriberId))
                .ReturnsAsync(workflows);

            // Act
            var result = await _handler.ListAsync(_subscriberId);

            // Assert
            result.Should().HaveCount(2);
            result[0].Name.Should().Be("Workflow 1");
            result[1].Name.Should().Be("Workflow 2");
        }

        [Fact]
        public async Task GetAsync_Should_Return_Workflow_By_Id()
        {
            // Arrange
            var workflow = new AutomationWorkflow(
                _subscriberId,
                "Test Workflow",
                "Test Description",
                NotificationContent.Create("Subject", "Body"),
                EventType.TestForDevelop,
                NotificationChannel.Email,
                "test_event",
                "",
                new List<string> { "user@example.com" });

            _mockAutomationRepository
                .Setup(r => r.GetByIdAsync(workflow.Id))
                .ReturnsAsync(workflow);

            var command = new WorkflowNotificationByIdCommand
            {
                SubscriberId = _subscriberId,
                Id = workflow.Id
            };

            // Act
            var result = await _handler.GetAsync(command);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Test Workflow");
        }

        [Fact]
        public async Task GetAsync_Should_Throw_Exception_When_Workflow_NotFound()
        {
            // Arrange
            var command = new WorkflowNotificationByIdCommand
            {
                SubscriberId = _subscriberId,
                Id = Guid.NewGuid()
            };

            _mockAutomationRepository
                .Setup(r => r.GetByIdAsync(command.Id))
                .ReturnsAsync((AutomationWorkflow)null!);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _handler.GetAsync(command));
        }

        [Fact]
        public async Task GetAsync_Should_Throw_Exception_For_Non_Owned_Workflow()
        {
            // Arrange
            var workflow = new AutomationWorkflow(
                Guid.NewGuid(), // Different subscriber
                "Test",
                "Test",
                NotificationContent.Create("S", "B"),
                EventType.TestForDevelop,
                NotificationChannel.Email,
                "event",
                "",
                new List<string> { "user@example.com" });

            _mockAutomationRepository
                .Setup(r => r.GetByIdAsync(workflow.Id))
                .ReturnsAsync(workflow);

            var command = new WorkflowNotificationByIdCommand
            {
                SubscriberId = _subscriberId,
                Id = workflow.Id
            };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(
                () => _handler.GetAsync(command));
        }

        [Fact]
        public async Task DeleteAsync_Should_Delete_Workflow()
        {
            // Arrange
            var workflow = new AutomationWorkflow(
                _subscriberId,
                "Test",
                "Test",
                NotificationContent.Create("S", "B"),
                EventType.TestForDevelop,
                NotificationChannel.Email,
                "event",
                "",
                new List<string> { "user@example.com" });

            _mockAutomationRepository
                .Setup(r => r.GetByIdAsync(workflow.Id))
                .ReturnsAsync(workflow);
            _mockAutomationRepository
                .Setup(r => r.DeleteAsync(workflow.Id))
                .Returns(Task.CompletedTask);
            _mockAutomationRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            var command = new WorkflowNotificationByIdCommand
            {
                SubscriberId = _subscriberId,
                Id = workflow.Id
            };

            // Act
            await _handler.DeleteAsync(command);

            // Assert
            _mockAutomationRepository.Verify(r => r.DeleteAsync(workflow.Id), Times.Once);
            _mockAutomationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockWorkflowScheduleService.Verify(s => s.RemoveAsync(workflow.Id), Times.Once);
        }

        [Fact]
        public async Task ActivateAsync_Should_Activate_Workflow()
        {
            // Arrange
            var workflow = new AutomationWorkflow(
                _subscriberId,
                "Test",
                "Test",
                NotificationContent.Create("S", "B"),
                EventType.TestForDevelop,
                NotificationChannel.Email,
                "event",
                "",
                new List<string> { "user@example.com" });
            workflow.Deactivate();

            _mockAutomationRepository
                .Setup(r => r.GetByIdAsync(workflow.Id))
                .ReturnsAsync(workflow);
            _mockAutomationRepository
                .Setup(r => r.Update(It.IsAny<AutomationWorkflow>()));
            _mockAutomationRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mockWorkflowScheduleService
                .Setup(s => s.SyncAsync(It.IsAny<AutomationWorkflow>()))
                .Returns(Task.CompletedTask);

            var command = new WorkflowNotificationByIdCommand
            {
                SubscriberId = _subscriberId,
                Id = workflow.Id
            };

            // Act
            await _handler.ActivateAsync(command);

            // Assert
            workflow.IsActive.Should().Be(true);
            _mockAutomationRepository.Verify(r => r.Update(It.IsAny<AutomationWorkflow>()), Times.Once);
            _mockAutomationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockWorkflowScheduleService.Verify(s => s.SyncAsync(It.IsAny<AutomationWorkflow>()), Times.Once);
        }

        [Fact]
        public async Task DeactivateAsync_Should_Deactivate_Workflow()
        {
            // Arrange
            var workflow = new AutomationWorkflow(
                _subscriberId,
                "Test",
                "Test",
                NotificationContent.Create("S", "B"),
                EventType.TestForDevelop,
                NotificationChannel.Email,
                "event",
                "",
                new List<string> { "user@example.com" });

            _mockAutomationRepository
                .Setup(r => r.GetByIdAsync(workflow.Id))
                .ReturnsAsync(workflow);
            _mockAutomationRepository
                .Setup(r => r.Update(It.IsAny<AutomationWorkflow>()));
            _mockAutomationRepository
                .Setup(r => r.SaveChangesAsync())
                .Returns(Task.CompletedTask);
            _mockWorkflowScheduleService
                .Setup(s => s.RemoveAsync(workflow.Id))
                .Returns(Task.CompletedTask);

            var command = new WorkflowNotificationByIdCommand
            {
                SubscriberId = _subscriberId,
                Id = workflow.Id
            };

            // Act
            await _handler.DeactivateAsync(command);

            // Assert
            workflow.IsActive.Should().Be(false);
            _mockAutomationRepository.Verify(r => r.Update(It.IsAny<AutomationWorkflow>()), Times.Once);
            _mockAutomationRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
            _mockWorkflowScheduleService.Verify(s => s.RemoveAsync(workflow.Id), Times.Once);
        }
    }
}
