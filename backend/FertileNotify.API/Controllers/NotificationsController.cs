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
            var subscriberId = GetSubscriberIdFromClaims();

            var eventType = EventType.From(request.EventType);
            var parameters = request.Parameters;

            int totalQueued = 0;

            foreach (var group in request.To)
            {
                var channel = NotificationChannel.From(group.Channel);
                foreach (var recipientAddress in group.Recipients)
                {
                    var command = new ProcessEventCommand
                    {
                        SubscriberId = subscriberId,
                        Channel = channel,
                        Recipient = recipientAddress.Trim(),
                        EventType = eventType,
                        Parameters = parameters
                    };

                    await _queue.QueueBackgroundWorkItemAsync(command);
                    totalQueued++;
                }
            }

            return Accepted(ApiResponse<object>.SuccessResult(
                new
                {
                    status = "Queued",
                    count = totalQueued
                },
                $"Total {totalQueued} notifications have been added to the queue.")
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
