using FertileNotify.API.Models.Requests;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.ForgotPassword;
using FertileNotify.Application.UseCases.Login;
using FertileNotify.Application.UseCases.VerifyCode;
using FertileNotify.Application.UseCases.RefreshToken;
using FertileNotify.Application.DTOs;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ITokenService _tokenService;
        private readonly LoginHandler _loginHandler;
        private readonly VerifyCodeHandler _verifyCodeHandler;
        private readonly ForgotPasswordHandler _forgotPasswordHandler;
        private readonly RefreshTokenHandler _refreshTokenHandler;

        public AuthController(
            ISubscriberRepository subscriberRepository,
            ISubscriptionRepository subscriptionRepository,
            ITokenService tokenService,
            LoginHandler loginHandler,
            VerifyCodeHandler verifyCodeHandler,
            ForgotPasswordHandler forgotPasswordHandler,
            RefreshTokenHandler refreshTokenHandler)
        {
            _subscriberRepository = subscriberRepository;
            _subscriptionRepository = subscriptionRepository;
            _tokenService = tokenService;
            _loginHandler = loginHandler;
            _verifyCodeHandler = verifyCodeHandler;
            _forgotPasswordHandler = forgotPasswordHandler;
            _refreshTokenHandler = refreshTokenHandler;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            await _loginHandler.HandleAsync(new LoginCommand { Email = request.Email, Password = request.Password });
            return Ok(ApiResponse<object>.SuccessResult(null!, "OTP sent to your email."));
        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] OtpRequest request)
        {
            var result = await _verifyCodeHandler.HandleAsync(new VerifyCodeCommand
            {
                Email = request.Email,
                Code = request.OtpCode
            });

            return Ok(ApiResponse<LoginResponseDto>.SuccessResult(result, "Login successful."));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            var result = await _refreshTokenHandler.HandleAsync(new RefreshTokenCommand { RefreshToken = request.RefreshToken });
            return Ok(ApiResponse<LoginResponseDto>.SuccessResult(result, "Token refreshed successfully."));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            await _forgotPasswordHandler.HandleAsync(new ForgotPasswordCommand { Email = request.Email });
            return Ok(ApiResponse<object>.SuccessResult(null!, "OTP sent to your email."));
        }

        [NonAction]
        public async Task<Subscriber> GetSubscriber(string email)
            => await _subscriberRepository.GetByEmailAsync(EmailAddress.Create(email))
                ?? throw new NotFoundException("Subscriber not found");
    }
}
