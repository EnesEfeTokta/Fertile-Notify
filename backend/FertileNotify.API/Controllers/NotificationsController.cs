using Microsoft.AspNetCore.Mvc;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.API.Models;
using FertileNotify.Domain.Events;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsController : ControllerBase
    {
        private readonly ProcessEventHandler _handler;

        public NotificationsController(ProcessEventHandler handler)
        {
            _handler = handler;
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

            try
            {
                await _handler.HandleAsync(command);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
