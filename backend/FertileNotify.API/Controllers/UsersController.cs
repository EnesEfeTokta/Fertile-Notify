using FertileNotify.Application.UseCases.CreateUserWithSubscription;
using FertileNotify.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly CreateUserHandler _createUserHandler;

        public UsersController(CreateUserHandler createUserHandler)
        {
            _createUserHandler = createUserHandler;
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateUserRequest request)
        {
            var userId = await _createUserHandler.HandleAsync(request.Email, request.Plan);
            return Ok(new { UserId = userId });
        }
    }

    public record CreateUserRequest(string Email, SubscriptionPlan Plan);
}
