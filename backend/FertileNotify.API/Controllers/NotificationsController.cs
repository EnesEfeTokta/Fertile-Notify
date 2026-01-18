using Microsoft.AspNetCore.Mvc;
using FertileNotify.Application.UseCases.ProcessEvent;

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
        public async Task<IActionResult> Send(ProcessEventCommand command)
        {
            await _handler.HandleAsync(command);
            return Ok();
        }
    }
}
