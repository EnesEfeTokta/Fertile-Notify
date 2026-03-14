using FertileNotify.API.Models.Requests;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.Contracts;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using MassTransit;
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
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IBlacklistRepository _blacklistRepository;
        private readonly ISecurityService _securityService;

        public NotificationsController(IPublishEndpoint publishEndpoint, IBlacklistRepository blacklistRepository, ISecurityService securityService)
        {
            _publishEndpoint = publishEndpoint;
            _blacklistRepository = blacklistRepository;
            _securityService = securityService;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
        {
            var subscriberId = GetSubscriberIdFromClaims();

            var eventType = EventType.From(request.EventType);
            byte priority = eventType.GetPriority();
            var parameters = request.Parameters;

            var allAddresses = request.To.SelectMany(g => g.Recipients).Distinct().ToList();
            var blacklistedItems = await _blacklistRepository.GetForRecipientsAsync(subscriberId, allAddresses);

            int totalQueued = 0;

            foreach (var group in request.To)
            {
                var channel = NotificationChannel.From(group.Channel);
                foreach (var recipientAddress in group.Recipients)
                {
                    var blacklistEntry = blacklistedItems.FirstOrDefault(b => b.RecipientAddress == recipientAddress);

                    if (blacklistEntry != null)
                    {
                        if (blacklistEntry.UnwantedChannels.Count == 0 ||
                            blacklistEntry.UnwantedChannels.Contains(channel))
                        {
                            continue;
                        }
                    }

                    await _publishEndpoint.Publish<ProcessNotificationMessage>(new
                    {
                        SubscriberId = subscriberId,
                        Channel = channel,
                        Recipient = recipientAddress,
                        EventType = request.EventType,
                        Parameters = request.Parameters
                    }, context => {
                        context.SetPriority(priority);
                    });
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

        [AllowAnonymous]
        [HttpPost("unsubscribe")]
        public async Task<IActionResult> PublicUnsubscribe([FromBody] UnsubscribeRequest request)
        {
            bool isValid = _securityService.VerifyUnsubscribeToken(request.Recipient, request.SubscriberId, request.Token);

            if (!isValid)
                return BadRequest(ApiResponse<object>.FailureResult(new List<string> { "Invalid unsubscribe token." }, "Failed to unsubscribe."));

            var forbiddenRecipient = new ForbiddenRecipient(
                request.SubscriberId, 
                request.Recipient,
                request.Channels?.Select(NotificationChannel.From).ToList() ?? new List<NotificationChannel>()
            );
            await _blacklistRepository.AddOrUpdateAsync(forbiddenRecipient);

            return Ok(ApiResponse<object>.SuccessResult(null!, "You have been successfully unsubscribed."));
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
