using FertileNotify.API.Models;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
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
        private readonly INotificationQueue _queue;

        public NotificationsController(INotificationQueue queue)
        {
            _queue = queue;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                ?? throw new UnauthorizedException("User ID claim not found.");

            var command = new ProcessEventCommand
            {
                SubscriberId = Guid.Parse(userIdClaim.Value),
                Recipient = request.Recipient,
                EventType = EventType.From(request.EventType),
                Parameters = request.Parameters
            };

            await _queue.QueueBackgroundWorkItemAsync(command);
            return Accepted((new { status = "Queued", message = "The notification has been added to the queue." }));
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkSend([FromBody] BulkNotificationRequest request)
        {
            var subscriberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("Subscriber ID claim not found.");

            var subscriberId = Guid.Parse(subscriberIdClaim.Value);

            var eventType = EventType.From(request.EventType);

            foreach (var recipient in request.Recipients)
            {
                var command = new ProcessEventCommand
                {
                    SubscriberId = subscriberId,
                    Recipient = recipient,
                    EventType = eventType,
                    Parameters = request.Parameters
                };

                await _queue.QueueBackgroundWorkItemAsync(command);
            }

            return Accepted(new
            {
                status = "Queued",
                totalRequested = request.Recipients.Count,
                message = $"{request.Recipients.Count} notifications have been added to the queue."
            });
        }
    }
}
