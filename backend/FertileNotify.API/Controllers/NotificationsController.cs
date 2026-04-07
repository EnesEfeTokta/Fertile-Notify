using FertileNotify.API.Models.Requests;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.UseCases.SendNotification;
using FertileNotify.Application.UseCases.Workflow;
using MediatR;
using FertileNotify.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FertileNotify.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(IMediator mediator, ILogger<NotificationsController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
        {
            var subscriberId = GetSubscriberIdFromClaims();

            var command = new SendNotificationCommand
            {
                SubscriberId = subscriberId,
                EventType = request.EventType,
                Parameters = request.Parameters,
                To = request.To.Select(g => new ChannelRecipientCommandGroup
                {
                    Channel = g.Channel,
                    Recipients = g.Recipients
                }).ToList()
            };

            int totalQueued = await _mediator.Send(command);

            return Accepted(ApiResponse<object>.SuccessResult(
                new
                {
                    status = "Queued",
                    count = totalQueued
                },
                $"Total {totalQueued} notifications have been added to the queue.")
            );
        }

        [HttpPost("workflow/send/{eventTrigger}")]
        public async Task<IActionResult> SendWorkflowNotification([FromRoute] string eventTrigger)
        {
            var subscriberId = GetSubscriberIdFromClaims();

            if (string.IsNullOrWhiteSpace(eventTrigger))
            {
                return BadRequest(ApiResponse<object>.FailureResult(new List<string> { "eventTrigger cannot be empty." }));
            }

            var totalQueued = await _mediator.Send(new TriggerWorkflowNotificationsCommand
            {
                SubscriberId = subscriberId,
                EventTrigger = eventTrigger
            });

            if (totalQueued == 0)
            {
                return NotFound(ApiResponse<object>.FailureResult(new List<string>
                {
                    $"No active workflow matched eventTrigger '{eventTrigger}'."
                }));
            }

            return Accepted(ApiResponse<object>.SuccessResult(
                new
                {
                    status = "Workflow notification queued",
                    count = totalQueued
                },
                $"Total {totalQueued} workflow notifications have been added to the queue.")
            );
        }

        [HttpPost("workflow/add")]
        public async Task<IActionResult> AddWorkflowNotification([FromBody] AddWorkflowNotificationRequest request)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var workflowId = await _mediator.Send(new CreateWorkflowNotificationCommand
            {
                SubscriberId = subscriberId,
                Name = request.Name,
                Description = request.Description,
                EventType = request.EventType,
                Channel = request.Channels,
                EventTrigger = request.EventTrigger,
                CronExpression = request.CronExpression,
                Subject = request.Subject,
                Body = request.Body,
                To = request.To.Select(x => new WorkflowRecipientGroupCommand
                {
                    Channel = x.Channel,
                    Recipients = x.Recipients
                }).ToList()
            });

            return Accepted(ApiResponse<object>.SuccessResult(
                new
                {
                    status = "Workflow notification created",
                    workflowId
                },
                "Workflow notification has been created and is waiting for trigger.")
            );
        }

        [HttpPut("workflow/update")]
        public async Task<IActionResult> UpdateWorkflowNotification([FromBody] UpdateWorkflowNotificationRequest request)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            await _mediator.Send(new UpdateWorkflowNotificationCommand
            {
                SubscriberId = subscriberId,
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                EventType = request.EventType,
                Channel = request.Channel,
                EventTrigger = request.EventTrigger,
                CronExpression = request.CronExpression,
                Subject = request.Subject,
                Body = request.Body,
                To = request.To?.Select(x => new WorkflowRecipientGroupCommand
                {
                    Channel = x.Channel,
                    Recipients = x.Recipients
                }).ToList()
            });

            return Accepted(ApiResponse<object>.SuccessResult(
                new
                {
                    status = "Workflow notification updated"
                },
                "Workflow notification has been updated.")
            );
        }

        [HttpGet("workflow/list")]
        public async Task<IActionResult> ListWorkflowNotifications()
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var workflows = await _mediator.Send(new ListWorkflowNotificationsQuery { SubscriberId = subscriberId });

            var data = workflows.Select(w => new
            {
                id = w.Id,
                name = w.Name,
                description = w.Description,
                channel = w.Channel,
                eventTrigger = w.EventTrigger,
                cronExpression = w.CronExpression,
                recipients = w.Recipients,
                isActive = w.IsActive,
                createdAt = w.CreatedAt,
                subject = w.Subject,
                body = w.Body
            });

            return Ok(ApiResponse<object>.SuccessResult(
                data,
                $"{workflows.Count} workflow notification(s) found."));
        }

        [HttpGet("workflow/get/{id}")]
        public async Task<IActionResult> GetWorkflowNotification(Guid id)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var workflow = await _mediator.Send(new GetWorkflowNotificationQuery
            {
                SubscriberId = subscriberId,
                Id = id
            });

            return Ok(ApiResponse<object>.SuccessResult(
                new
                {
                    id = workflow.Id,
                    name = workflow.Name,
                    description = workflow.Description,
                    channel = workflow.Channel,
                    eventTrigger = workflow.EventTrigger,
                    cronExpression = workflow.CronExpression,
                    recipients = workflow.Recipients,
                    isActive = workflow.IsActive,
                    createdAt = workflow.CreatedAt,
                    subject = workflow.Subject,
                    body = workflow.Body
                },
                "Workflow notification found."));
        }

        [HttpDelete("workflow/delete/{id}")]
        public async Task<IActionResult> DeleteWorkflowNotification(Guid id)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            await _mediator.Send(new DeleteWorkflowNotificationCommand
            {
                SubscriberId = subscriberId,
                Id = id
            });

            return Accepted(ApiResponse<object>.SuccessResult(
                new
                {
                    status = "Workflow notification deleted"
                },
                "Workflow notification has been deleted.")
            );
        }

        [HttpPost("workflow/activate/{id}")]
        public async Task<IActionResult> ActivateWorkflowNotification(Guid id)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            await _mediator.Send(new ActivateWorkflowNotificationCommand
            {
                SubscriberId = subscriberId,
                Id = id
            });

            return Accepted(ApiResponse<object>.SuccessResult(
                new
                {
                    status = "Workflow notification activated"
                },
                "Workflow notification has been activated.")
            );
        }

        [HttpPost("workflow/deactivate/{id}")]
        public async Task<IActionResult> DeactivateWorkflowNotification(Guid id)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            await _mediator.Send(new DeactivateWorkflowNotificationCommand
            {
                SubscriberId = subscriberId,
                Id = id
            });

            return Accepted(ApiResponse<object>.SuccessResult(
                new
                {
                    status = "Workflow notification deactivated"
                },
                "Workflow notification has been deactivated.")
            );
        }

        [NonAction]
        private Guid GetSubscriberIdFromClaims()
        {
            var subscriberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("Subscriber ID claim not found.");
            return Guid.Parse(subscriberIdClaim.Value);
        }
    }
}
