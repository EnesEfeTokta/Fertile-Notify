using FertileNotify.API.Models;
using FertileNotify.Application.UseCases.RegisterSubscriber;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class SubscriberController : ControllerBase
    {
        private readonly RegisterSubscriberHandler _registerUserHandler;

        public SubscriberController(RegisterSubscriberHandler registerUserHandler)
        {
            _registerUserHandler = registerUserHandler;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterSubscriberRequest request)
        {
            Enum.TryParse<SubscriptionPlan>(request.Plan, ignoreCase: true, out var plan);
            var command = new RegisterSubscriberCommand
            {
                Email = EmailAddress.Create(request.Email),
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                                ? null
                                : PhoneNumber.Create(request.PhoneNumber),
                Plan = plan,
            };
            var userId = await _registerUserHandler.HandleAsync(command);
            return CreatedAtAction(nameof(Register), new { id = userId }, new { UserId = userId });
        }
    }
}