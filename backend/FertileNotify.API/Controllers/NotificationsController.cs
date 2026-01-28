using Microsoft.AspNetCore.Mvc;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.API.Models;
using FertileNotify.Domain.Events;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FertileNotify.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationQueue _queue;
        private readonly IUserRepository _userRepository;

        public NotificationsController(INotificationQueue queue, IUserRepository userRepository)
        {
            _queue = queue;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Send([FromBody] SendNotificationRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                ?? throw new UnauthorizedException("User ID claim not found.");

            var command = new ProcessEventCommand
            {
                UserId = Guid.Parse(userIdClaim.Value),
                EventType = EventType.From(request.EventType),
                Parameters = request.Parameters
            };

            await _queue.QueueBackgroundWorkItemAsync(command);
            return Accepted((new { status = "Queued", message = "The notification has been added to the queue." }));
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkSend([FromBody] BulkNotificationRequest request)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier) 
                ?? throw new UnauthorizedException("User ID claim not found.");

            var existingIds = await _userRepository.GetExistingIdsAsync(request.UserIds)
                ?? throw new UnauthorizedException("None of the provided IDs are registered in the system.");

            foreach (var userId in existingIds)
            {
                var command = new ProcessEventCommand
                {
                    UserId = userId,
                    EventType = EventType.From(request.EventType),
                    Parameters = request.Parameters
                };
                await _queue.QueueBackgroundWorkItemAsync(command);
            }

            return Accepted(new 
            { 
                status = "Queued",
                totalRequested = request.UserIds.Count,
                queuedCount = existingIds.Count,
                message = "The bulk notifications have been added to the queue." 
            }); 
        }
    }
}
