using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using FertileNotify.Application.DTOs;

namespace FertileNotify.Application.UseCases.Workflow
{
    public class WorkflowNotificationHandler
    {
        private readonly IAutomationRepository _automationRepository;
        private readonly AutomationTriggerService _automationTriggerService;

        public WorkflowNotificationHandler(
            IAutomationRepository automationRepository,
            AutomationTriggerService automationTriggerService)
        {
            _automationRepository = automationRepository;
            _automationTriggerService = automationTriggerService;
        }

        public async Task<int> TriggerAsync(TriggerWorkflowNotificationsCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.EventTrigger))
            {
                throw new BusinessRuleException("eventTrigger cannot be empty.");
            }

            return await _automationTriggerService.TriggerWorkflowsAsync(command.SubscriberId, command.EventTrigger.Trim());
        }

        public async Task<Guid> CreateAsync(CreateWorkflowNotificationCommand command)
        {
            var recipients = CollectRecipients(command.To);
            if (recipients.Count == 0)
            {
                throw new BusinessRuleException("At least one recipient is required.");
            }

            if (string.IsNullOrWhiteSpace(command.EventTrigger))
            {
                throw new BusinessRuleException("EventTrigger is required.");
            }

            var resolvedChannel = ResolveChannel(command.Channel, command.To);
            var channel = NotificationChannel.From(resolvedChannel);
            var content = NotificationContent.Create(command.Subject, command.Body);

            var workflow = new AutomationWorkflow(
                command.SubscriberId,
                command.Name,
                command.Description,
                content,
                channel,
                command.EventTrigger.Trim(),
                command.CronExpression,
                recipients);

            await _automationRepository.CreateAsync(workflow);
            await _automationRepository.SaveChangesAsync();

            return workflow.Id;
        }

        public async Task UpdateAsync(UpdateWorkflowNotificationCommand command)
        {
            var workflow = await GetOwnedWorkflowAsync(command.SubscriberId, command.Id);

            if (!string.IsNullOrWhiteSpace(command.Name) || !string.IsNullOrWhiteSpace(command.Description))
            {
                workflow.UpdateDetails(command.Name ?? workflow.Name, command.Description ?? workflow.Description);
            }

            if (!string.IsNullOrWhiteSpace(command.Subject) || !string.IsNullOrWhiteSpace(command.Body))
            {
                var currentContent = workflow.Content;
                var updatedContent = NotificationContent.Create(
                    command.Subject ?? currentContent.Subject,
                    command.Body ?? currentContent.Body);
                workflow.UpdateContent(updatedContent);
            }

            if (!string.IsNullOrWhiteSpace(command.Channel))
            {
                workflow.UpdateChannel(NotificationChannel.From(command.Channel));
            }

            if (command.To != null && command.To.Count > 0)
            {
                var recipients = CollectRecipients(command.To);
                if (recipients.Count > 0)
                {
                    workflow.UpdateRecipients(recipients);
                }
            }

            if (!string.IsNullOrWhiteSpace(command.EventTrigger) || !string.IsNullOrWhiteSpace(command.CronExpression))
            {
                workflow.UpdateSchedule(
                    command.EventTrigger ?? workflow.EventTrigger,
                    command.CronExpression ?? workflow.CronExpression);
            }

            _automationRepository.UpdateAsync(workflow);
            await _automationRepository.SaveChangesAsync();
        }

        public async Task<List<WorkflowNotificationDto>> ListAsync(Guid subscriberId)
        {
            var workflows = await _automationRepository.GetBySubscriberIdAsync(subscriberId);
            return workflows.Select(MapToDto).ToList();
        }

        public async Task<WorkflowNotificationDto> GetAsync(WorkflowNotificationByIdCommand command)
        {
            var workflow = await GetOwnedWorkflowAsync(command.SubscriberId, command.Id);
            return MapToDto(workflow);
        }

        public async Task DeleteAsync(WorkflowNotificationByIdCommand command)
        {
            await GetOwnedWorkflowAsync(command.SubscriberId, command.Id);
            await _automationRepository.DeleteAsync(command.Id);
            await _automationRepository.SaveChangesAsync();
        }

        public async Task ActivateAsync(WorkflowNotificationByIdCommand command)
        {
            var workflow = await GetOwnedWorkflowAsync(command.SubscriberId, command.Id);
            workflow.Activate();
            _automationRepository.UpdateAsync(workflow);
            await _automationRepository.SaveChangesAsync();
        }

        public async Task DeactivateAsync(WorkflowNotificationByIdCommand command)
        {
            var workflow = await GetOwnedWorkflowAsync(command.SubscriberId, command.Id);
            workflow.Deactivate();
            _automationRepository.UpdateAsync(workflow);
            await _automationRepository.SaveChangesAsync();
        }

        private async Task<AutomationWorkflow> GetOwnedWorkflowAsync(Guid subscriberId, Guid workflowId)
        {
            var workflow = await _automationRepository.GetByIdAsync(workflowId);
            if (workflow == null || workflow.SubscriberId != subscriberId)
            {
                throw new NotFoundException("Workflow not found.");
            }

            return workflow;
        }

        private static string ResolveChannel(string? commandChannel, List<WorkflowRecipientGroupCommand> recipients)
        {
            if (!string.IsNullOrWhiteSpace(commandChannel))
            {
                return commandChannel;
            }

            var firstChannel = recipients.FirstOrDefault()?.Channel;
            if (string.IsNullOrWhiteSpace(firstChannel))
            {
                throw new BusinessRuleException("Channel is required.");
            }

            return firstChannel;
        }

        private static List<string> CollectRecipients(List<WorkflowRecipientGroupCommand> groups)
        {
            return groups
                .SelectMany(x => x.Recipients)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private static WorkflowNotificationDto MapToDto(AutomationWorkflow workflow)
        {
            return new WorkflowNotificationDto
            {
                Id = workflow.Id,
                Name = workflow.Name,
                Description = workflow.Description,
                Channel = workflow.Channel.Name,
                EventTrigger = workflow.EventTrigger,
                CronExpression = workflow.CronExpression,
                Recipients = workflow.Recipients,
                IsActive = workflow.IsActive,
                CreatedAt = workflow.CreatedAt,
                Subject = workflow.Content.Subject,
                Body = workflow.Content.Body
            };
        }
    }
}
