using Microsoft.AspNetCore.Mvc;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.API.Models;
using FertileNotify.Domain.Events;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Exceptions;

namespace FertileNotify.API.Controllers
{
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
            if (!await _userRepository.ExistsAsync(request.UserId)) 
                throw new NotFoundException("User not found");

            var command = new ProcessEventCommand
            {
                UserId = request.UserId,
                EventType = EventType.From(request.EventType),
                Parameters = request.Parameters
            };

            await _queue.QueueBackgroundWorkItemAsync(command);
            return Accepted((new { status = "Queued", message = "The notification has been added to the queue." }));
        }
    }
}
