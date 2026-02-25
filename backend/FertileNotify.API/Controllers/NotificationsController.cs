using FertileNotify.API.Models.Requests;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
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
        private readonly INotificationQueue _queue;

        public NotificationsController(INotificationQueue queue)
        {
            _queue = queue;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
        {
            var command = new ProcessEventCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                Channel = NotificationChannel.From(request.Channel.Trim().ToLower()),
                Recipient = request.Recipient.Trim().ToLower(),
                EventType = EventType.From(request.EventType),
                Parameters = request.Parameters
            };

            await _queue.QueueBackgroundWorkItemAsync(command);
            return Accepted(ApiResponse<object>.SuccessResult(new { status = "Queued" }, "The notification has been added to the queue."));
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkSend([FromBody] SendBulkNotificationRequest request)
        {
            Guid id = GetSubscriberIdFromClaims();

            foreach (var recipient in request.Recipients)
            {
                var command = new ProcessEventCommand
                {
                    SubscriberId = id,
                    Channel = NotificationChannel.From(request.Channel),
                    Recipient = recipient,
                    EventType = EventType.From(request.EventType),
                    Parameters = request.Parameters
                };

                await _queue.QueueBackgroundWorkItemAsync(command);
            }

            return Accepted(ApiResponse<object>.SuccessResult(new
            {
                status = "Queued",
                totalRequested = request.Recipients.Count
            }, $"{request.Recipients.Count} notifications have been added to the queue."));
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
