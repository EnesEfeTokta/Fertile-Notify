using System;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public AuthController(IUserRepository userRepository, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = 
                await _userRepository.GetByIdAsync(request.UserId)
                ?? throw new NotFoundException("User not found");

            var token = _tokenService.GenerateToken(user);

            return Ok( new { Token = token } );
        }
    }

    public class LoginRequest
    {
        public Guid UserId { get; set; } // ! Kullanıcılar bellekte tutuluyor. Bu yüzden şimdilik Guid ile kullanıcı bulunsun.
    }
}
