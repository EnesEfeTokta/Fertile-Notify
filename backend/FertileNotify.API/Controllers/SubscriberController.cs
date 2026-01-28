using FertileNotify.API.Models;
using FertileNotify.Application.UseCases.RegisterSubscriber;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/subscribers")]
    public class SubscriberController : ControllerBase
    {
        private readonly RegisterSubscriberHandler _registerSubscriberHandler;

        public SubscriberController(RegisterSubscriberHandler registerSubscriberHandler)
        {
            _registerSubscriberHandler = registerSubscriberHandler;
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
            await _registerSubscriberHandler.HandleAsync(command);
            return CreatedAtAction(nameof(Register), new { message = "Registration successful, log in." });
        }
    }
}