using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using FertileNotify.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISubscriberRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthController(ISubscriberRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user =
                await _userRepository.GetByEmailAsync(EmailAddress.Create(request.Email))
                ?? throw new NotFoundException("User not found");

            var token = _tokenService.GenerateToken(user);

            return Ok( new { Token = token } );
        }
    }
}
