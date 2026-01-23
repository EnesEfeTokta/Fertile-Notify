using Microsoft.AspNetCore.Mvc;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.API.Models;
using FertileNotify.Domain.Events;
using FertileNotify.Application.Interfaces;

namespace FertileNotify.API.Controllers
{
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
            if (request == null)
                return BadRequest(new { error = "Request body is required." });

            EventType eventType;
            try { eventType = EventType.From(request.EventType); }
            catch { return BadRequest(new { error = $"Invalid event type: {request.EventType}" }); }

            var command = new ProcessEventCommand
            {
                UserId = request.UserId,
                EventType = eventType,
                Parameters = request.Parameters
            };

            await _queue.QueueBackgroundWorkItemAsync(command);

            return Accepted((new { status = "Queued", message = "The notification has been added to the queue." }));
        }
    }
}
