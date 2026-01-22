using FertileNotify.Application.UseCases.RegisterUser;
using Microsoft.AspNetCore.Mvc;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly RegisterUserHandler _registerUserHandler;

        public UsersController(RegisterUserHandler registerUserHandler)
        {
            _registerUserHandler = registerUserHandler;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            var userId = await _registerUserHandler.HandleAsync(command);
            return CreatedAtAction(nameof(Register), new { id = userId }, new { UserId = userId });
        }
    }
}