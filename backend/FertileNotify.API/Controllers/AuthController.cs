using FertileNotify.API.Models.Requests;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using Microsoft.AspNetCore.Identity.Data;
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
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;

        public AuthController(
            ISubscriberRepository subscriberRepository,
            ISubscriptionRepository subscriptionRepository,
            ITokenService tokenService, 
            IOtpService otpService,
            IEmailService emailService)
        {
            _subscriberRepository = subscriberRepository;
            _subscriptionRepository = subscriptionRepository;
            _tokenService = tokenService;
            _otpService = otpService;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var subscriber = await GetSubscriber(request.Email);

            if (!subscriber.Password.Verify(request.Password))
                throw new UnauthorizedException("Invalid credentials");

            var otpCode = await _otpService.GenerateOtpAsync(subscriber.Id);
            await _emailService.SendEmailAsync(subscriber.Email.ToString(), "OTP Verification", $"Your OTP is: {otpCode}.");

            return Ok(ApiResponse<object>.SuccessResult(default!, "A special 6-character code valid for 5 minutes has been sent to your email address. Please enter this code."));

        }

        [HttpPost("verify-code")]
        public async Task<IActionResult> VerifyCode([FromBody] OtpRequest request)
        {
            var subscriber = await GetSubscriber(request.Email);

            var isValid = await _otpService.VerifyOtpAsync(subscriber.Id, request.OtpCode);

            if (!isValid)
                throw new UnauthorizedException("Invalid or expired OTP code");

            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(subscriber.Id)
                                ?? throw new NotFoundException("Subscription not found");

            var accessToken = _tokenService.GenerateToken(subscriber, subscription.Plan);
            var refreshToken = _tokenService.GenerateRefreshToken();

            subscriber.SetRefreshToken(refreshToken);
            await _subscriberRepository.SaveAsync(subscriber);

            return Ok(ApiResponse<object>.SuccessResult(new { AccessToken = accessToken, RefreshToken = refreshToken }, "Login successful."));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken_([FromBody] RefreshTokenRequest request)
        {
            var subscriber = await _subscriberRepository.GetByRefreshTokenAsync(request.RefreshToken)
                                ?? throw new UnauthorizedException("Invalid or expired refresh token");

            if (!subscriber.RefreshToken!.IsActive())
                throw new UnauthorizedException("Refresh token has expired");

            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(subscriber.Id)
                    ?? throw new NotFoundException("Subscription not found");

            var newAccessToken = _tokenService.GenerateToken(subscriber, subscription.Plan);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            subscriber.SetRefreshToken(newRefreshToken);
            await _subscriberRepository.SaveAsync(subscriber);

            return Ok(ApiResponse<object>.SuccessResult(new { AccessToken = newAccessToken, RefreshToken = newRefreshToken }, "Token refreshed successfully."));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string email)
        {
            var subscriber = await GetSubscriber(email);
            var otpCode = await _otpService.GenerateOtpAsync(subscriber.Id);
            await _emailService.SendEmailAsync(subscriber.Email.ToString(), "Password Reset OTP", $"Your OTP for password reset is: {otpCode}.");
            return Ok(ApiResponse<object>.SuccessResult(default!, "A special 6-character code valid for 5 minutes has been sent to your email address. Please enter this code to reset your password."));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] UserResetPasswordRequest request)
        {
            var subscriber = await GetSubscriber(request.Email);
            var isValid = await _otpService.VerifyOtpAsync(subscriber.Id, request.OtpCode);
            if (!isValid)
                throw new UnauthorizedException("Invalid or expired OTP code");
            subscriber.UpdatePassword(Password.Create(request.NewPassword));
            await _subscriberRepository.SaveAsync(subscriber);
            return Ok(ApiResponse<object>.SuccessResult(default!, "Password reset successful."));
        }

        [NonAction]
        public async Task<Subscriber> GetSubscriber(string email)
            => await _subscriberRepository.GetByEmailAsync(EmailAddress.Create(email))
                ?? throw new NotFoundException("Subscriber not found");
    }
}
