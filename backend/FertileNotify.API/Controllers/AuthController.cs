using FertileNotify.API.Models;
using FertileNotify.Application.Interfaces;
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
        private readonly IOtpService _otpService;

        public AuthController(
            ISubscriberRepository subscriberRepository,
            ISubscriptionRepository subscriptionRepository,
            ITokenService tokenService, 
            IOtpService otpService)
        {
            _subscriberRepository = subscriberRepository;
            _subscriptionRepository = subscriptionRepository;
            _tokenService = tokenService;
            _otpService = otpService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var subscriber = await GetSubscriber(request.Email);

            if (!subscriber.Password.Verify(request.Password))
                throw new UnauthorizedException("Invalid credentials");

            var otpCode = await _otpService.GenerateOtpAsync(subscriber.Id);

            return Ok(new { Message = "A special 6-character code valid for 5 minutes " +
                                      "has been sent to your email address. Please enter this code." });
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

            var token = _tokenService.GenerateToken(subscriber, subscription.Plan);
            return Ok( new { Token = token } );
        }

        public async Task<Subscriber> GetSubscriber(string email)
            => await _subscriberRepository.GetByEmailAsync(EmailAddress.Create(email))
                ?? throw new NotFoundException("Subscriber not found");
    }
}
