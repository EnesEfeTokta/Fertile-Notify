using FertileNotify.API.Models;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.RegisterSubscriber;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/subscribers")]
    public class SubscriberController : ControllerBase
    {
        private readonly RegisterSubscriberHandler _registerSubscriberHandler;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public SubscriberController(
            RegisterSubscriberHandler registerSubscriberHandler,
            ISubscriptionRepository subscriptionRepository,
            ISubscriberRepository subscriberRepository)
        {
            _registerSubscriberHandler = registerSubscriberHandler;
            _subscriptionRepository = subscriptionRepository;
            _subscriberRepository = subscriberRepository;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var subscriber = await GetSubscriberAsync();

            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(subscriber.Id);

            var respone = new SubscriberDto
            {
                Id = GetSubscriberIdFromClaims(),
                CompanyName = subscriber.CompanyName.Name,
                Email = subscriber.Email.Value,
                PhoneNumber = subscriber.PhoneNumber?.Value,
                ActiveChannels = subscriber.ActiveChannels.Select(c => c.Name).ToList(),
                Subscription = subscription == null ? null : new SubscriptionDto
                {
                    Plan = subscription.Plan.ToString(),
                    MonthlyLimit = subscription.MonthlyLimit,
                    UsedThisMonth = subscription.UsedThisMonth,
                    ExpiresAt = subscription.ExpiresAt,
                }
            };

            return Ok(respone);
        }

        [Authorize]
        [HttpPut("contact")]
        public async Task<IActionResult> UpdateContactInfo([FromBody] UpdateContactRequest request)
        {
            var subscriber = await GetSubscriberAsync();

            subscriber.UpdateContactInfo(
                EmailAddress.Create(request.Email),
                string.IsNullOrWhiteSpace(request.PhoneNumber)
                    ? null
                    : PhoneNumber.Create(request.PhoneNumber)
            );

            await _subscriberRepository.SaveAsync(subscriber);
            return Ok();
        }

        [Authorize]
        [HttpPut("company-name")]
        public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyNameRequest request)
        {
            var subscriber = await GetSubscriberAsync();

            subscriber.UpdateCompanyName(CompanyName.Create(request.CompanyName));

            await _subscriberRepository.SaveAsync(subscriber);
            return Ok();
        }

        [Authorize]
        [HttpPost("channels")]
        public async Task<IActionResult> UpdateChannels([FromBody] ManageChannelRequest request)
        {
            var subscriber = await GetSubscriberAsync();

            var channel = NotificationChannel.From(request.Channel.Trim().ToLower());
            if (request.Enable)
                subscriber.EnableChannel(channel);
            else
                subscriber.DisableChannel(channel);

            await _subscriberRepository.SaveAsync(subscriber);
            return Ok(new { activeChannels = subscriber.ActiveChannels.Select(c => c.Name) });
        }

        [Authorize]
        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            var subscriber = await GetSubscriberAsync();

            if (!subscriber.Password.Verify(request.CurrentPassword))
                throw new UnauthorizedException("Current password is incorrect.");

            subscriber.UpdatePassword(Password.Create(request.NewPassword));

            await _subscriberRepository.SaveAsync(subscriber);
            return Ok();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterSubscriberRequest request)
        {
            Enum.TryParse<SubscriptionPlan>(request.Plan, ignoreCase: true, out var plan);
            var command = new RegisterSubscriberCommand
            {
                CompanyName = CompanyName.Create(request.CompanyName),
                Password = Password.Create(request.Password),
                Email = EmailAddress.Create(request.Email),
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                                ? null
                                : PhoneNumber.Create(request.PhoneNumber),
                Plan = plan,
            };
            await _registerSubscriberHandler.HandleAsync(command);
            return CreatedAtAction(nameof(Register), new { message = "Registration successful, log in." });
        }

        private Guid GetSubscriberIdFromClaims()
        {
            var subscriberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("Subscriber ID claim not found.");
            return Guid.Parse(subscriberIdClaim.Value);
        }

        private async Task<Subscriber> GetSubscriberAsync()
            => await _subscriberRepository.GetByIdAsync(GetSubscriberIdFromClaims())
                ?? throw new NotFoundException("Subscriber not found.");
    }
}