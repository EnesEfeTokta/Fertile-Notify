namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            await _mediator.Send(new LoginCommand { Email = request.Email, Password = request.Password });
            return Ok(ApiResponse<object>.SuccessResult(null!, "OTP sent to your email."));
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] OtpRequest request)
        {
            var result = await _mediator.Send(new VerifyCodeCommand
            {
                Email = request.Email,
                Code = request.OtpCode
            });

            return Ok(ApiResponse<LoginResponseDto>.SuccessResult(result, "Login successful."));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _mediator.Send(new RefreshTokenCommand { RefreshToken = request.RefreshToken });
            return Ok(ApiResponse<LoginResponseDto>.SuccessResult(result, "Token refreshed successfully."));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await _mediator.Send(new ForgotPasswordCommand { Email = request.Email });
            return Ok(ApiResponse<object>.SuccessResult(null!, "OTP sent to your email."));
        }

    }
}
