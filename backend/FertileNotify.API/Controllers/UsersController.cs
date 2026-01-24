using FertileNotify.API.Models;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.RegisterUser;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

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
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            Enum.TryParse<SubscriptionPlan>(request.Plan, ignoreCase: true, out var plan);
            var command = new RegisterUserCommand
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