using FertileNotify.API.Models.Requests;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.Services;
using FertileNotify.Application.UseCases.SendNotification;
using FertileNotify.Application.UseCases.Unsubscribe;
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
        private readonly SendNotificationHandler _sendNotificationHandler;
        private readonly UnsubscribeHandler _unsubscribeHandler;

        public NotificationsController(SendNotificationHandler sendNotificationHandler, UnsubscribeHandler unsubscribeHandler)
        {
            _sendNotificationHandler = sendNotificationHandler;
            _unsubscribeHandler = unsubscribeHandler;
        }

        [HttpPost]
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

            int totalQueued = await _sendNotificationHandler.HandleAsync(command);

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
