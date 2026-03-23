using FertileNotify.API.Models.Requests;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.DTOs;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.CreateApiKey;
using FertileNotify.Application.UseCases.ManageChannels;
using FertileNotify.Application.UseCases.RegisterSubscriber;
using FertileNotify.Application.UseCases.RevokeApiKey;
using FertileNotify.Application.UseCases.SetChannelSetting;
using FertileNotify.Application.UseCases.UpdateCompanyName;
using FertileNotify.Application.UseCases.UpdateContactInfo;
using FertileNotify.Application.UseCases.UpdatePassword;
using FertileNotify.Application.UseCases.DeleteAccount;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Enums;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FertileNotify.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/subscribers")]
    public class SubscriberController : ControllerBase
    {
        private readonly RegisterSubscriberHandler _registerSubscriberHandler;
        private readonly UpdateContactInfoHandler _updateContactInfoHandler;
        private readonly UpdateCompanyNameHandler _updateCompanyNameHandler;
        private readonly ManageChannelsHandler _manageChannelsHandler;
        private readonly UpdatePasswordHandler _updatePasswordHandler;
        private readonly CreateApiKeyHandler _createApiKeyHandler;
        private readonly RevokeApiKeyHandler _revokeApiKeyHandler;
        private readonly SetChannelSettingHandler _setChannelSettingHandler;
        private readonly DeleteAccountHandler _deleteAccountHandler;

        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;

        public SubscriberController(
            RegisterSubscriberHandler registerSubscriberHandler,
            UpdateContactInfoHandler updateContactInfoHandler,
            UpdateCompanyNameHandler updateCompanyNameHandler,
            ManageChannelsHandler manageChannelsHandler,
            UpdatePasswordHandler updatePasswordHandler,
            CreateApiKeyHandler createApiKeyHandler,
            RevokeApiKeyHandler revokeApiKeyHandler,
            SetChannelSettingHandler setChannelSettingHandler,
            DeleteAccountHandler deleteAccountHandler,
            ISubscriptionRepository subscriptionRepository,
            ISubscriberRepository subscriberRepository,
            IApiKeyRepository apiKeyRepository,
            ISubscriberChannelRepository subscriberChannelRepository)
        {
            _registerSubscriberHandler = registerSubscriberHandler;
            _updateContactInfoHandler = updateContactInfoHandler;
            _updateCompanyNameHandler = updateCompanyNameHandler;
            _manageChannelsHandler = manageChannelsHandler;
            _updatePasswordHandler = updatePasswordHandler;
            _createApiKeyHandler = createApiKeyHandler;
            _revokeApiKeyHandler = revokeApiKeyHandler;
            _setChannelSettingHandler = setChannelSettingHandler;
            _deleteAccountHandler = deleteAccountHandler;

            _subscriptionRepository = subscriptionRepository;
            _subscriberRepository = subscriberRepository;
            _apiKeyRepository = apiKeyRepository;
            _subscriberChannelRepository = subscriberChannelRepository;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyProfile()
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var subscriber = await _subscriberRepository.GetByIdAsync(subscriberId)
                ?? throw new NotFoundException("Subscriber not found.");

            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(subscriber.Id);

            var response = new SubscriberDto
            {
                CompanyName = subscriber.CompanyName.Name,
                Email = subscriber.Email.Value,
                PhoneNumber = subscriber.PhoneNumber?.Value,
                ActiveChannels = subscriber.ActiveChannels.Select(c => c.Name).ToList(),
                ExtraCredits = subscriber.ExtraCredits,
                Subscription = subscription == null ? null : new SubscriptionDto
                {
                    Plan = subscription.Plan.ToString(),
                    MonthlyLimit = subscription.MonthlyLimit,
                    UsedThisMonth = subscription.UsedThisMonth,
                    ExpiresAt = subscription.ExpiresAt,
                }
            };

            return Ok(ApiResponse<SubscriberDto>.SuccessResult(response, "Subscription information belonging to the subscriber."));
        }

        [HttpPut("contact")]
        public async Task<IActionResult> UpdateContactInfo([FromBody] UpdateContactRequest request)
        {
            await _updateContactInfoHandler.HandleAsync(new UpdateContactInfoCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                Email = request.Email,
                PhoneNumber = request.PhoneNumber
            });
            return Ok(ApiResponse<object>.SuccessResult(default!, "The subscriber's contact information has been updated."));
        }

        [HttpPut("company-name")]
        public async Task<IActionResult> UpdateCompany([FromBody] UpdateCompanyNameRequest request)
        {
            await _updateCompanyNameHandler.HandleAsync(new UpdateCompanyNameCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                CompanyName = request.CompanyName
            });
            return Ok(ApiResponse<object>.SuccessResult(default!, "The subscriber's company name has been updated."));
        }

        [HttpPost("channels")]
        public async Task<IActionResult> UpdateChannels([FromBody] ManageChannelRequest request)
        {
            await _manageChannelsHandler.HandleAsync(new ManageChannelsCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                Channel = request.Channel,
                Enable = request.Enable
            });
            return Ok(ApiResponse<object>.SuccessResult(default!, "The subscriber's channel settings have been updated."));
        }

        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            var command = new UpdatePasswordCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            };
            await _updatePasswordHandler.HandleAsync(command);
            return Ok(ApiResponse<object>.SuccessResult(default!, "The subscriber's password has been updated."));
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            await _deleteAccountHandler.HandleAsync(new DeleteAccountCommand
            {
                SubscriberId = GetSubscriberIdFromClaims()
            });
            return Ok(ApiResponse<object>.SuccessResult(default!, "The subscriber's account has been deleted."));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterSubscriberRequest request)
        {
            Enum.TryParse<SubscriptionPlan>(request.Plan, ignoreCase: true, out var plan);
            await _registerSubscriberHandler.HandleAsync(new RegisterSubscriberCommand
            {
                CompanyName = CompanyName.Create(request.CompanyName),
                Password = Password.Create(request.Password),
                Email = EmailAddress.Create(request.Email),
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                                ? null
                                : PhoneNumber.Create(request.PhoneNumber),
                Plan = plan,
            });
            return Ok(ApiResponse<RegisterSubscriberCommand>.SuccessResult(default!, "Registration successful, log in."));
        }

        [HttpPost("create-api-key")]
        public async Task<IActionResult> Create([FromBody] CreateApiKeyRequest request)
        {
            var command = new CreateApiKeyCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                Name = request.Name
            };
            var rawApiKey = await _createApiKeyHandler.HandleAsync(command);
            return Ok(ApiResponse<object>.SuccessResult(new { ApiKey = rawApiKey }, "Please save this key securely. You won't be able to see it again."));
        }

        [HttpGet("api-keys")]
        public async Task<IActionResult> GetApiKeys()
        {
            var apiKeys = await _apiKeyRepository.GetBySubscriberIdAsync(GetSubscriberIdFromClaims());
            var response = apiKeys.Select(k => new ApiKeyDto
            {
                Id = k.Id,
                Name = k.Name,
                Prefix = k.Prefix,
                IsActive = k.IsActive,
                CreatedAt = k.CreatedAt,
            });
            return Ok(ApiResponse<IEnumerable<ApiKeyDto>>.SuccessResult(response, "An API Key has been generated for the subscriber."));
        }

        [HttpDelete("api-keys/{apiKeyId}")]
        public async Task<IActionResult> RevokeApiKey(Guid apiKeyId)
        {
            await _revokeApiKeyHandler.HandleAsync(new RevokeApiKeyCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                ApiKeyId = apiKeyId
            });
            return Ok(ApiResponse<object>.SuccessResult(default!, "The subscriber's API Key has been decommissioned."));
        }

        [HttpPost("settings/channel-setting")]
        public async Task<IActionResult> SetChannelSetting([FromBody] ChannelSettingRequest request)
        {
            await _setChannelSettingHandler.HandleAsync(new SetChannelSettingCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                Channel = request.Channel,
                Settings = request.Settings
            });
            return Ok(ApiResponse<object>.SuccessResult(default!, $"{request.Channel} configured successfully."));
        }

        [HttpGet("settings/channel-setting")]
        public async Task<IActionResult> GetChannelSetting([FromQuery] string channel)
        {
            var setting = await _subscriberChannelRepository.GetSettingAsync(GetSubscriberIdFromClaims(), NotificationChannel.From(channel));

            if (setting == null)
                return NotFound(new { message = $"No settings found for {channel}." });

            return Ok(ApiResponse<SubscriberChannelSetting>.SuccessResult(setting, $"{channel} settings retrieved successfully."));
        }

        [HttpPatch("add-extra-credits")]
        public async Task<IActionResult> AddExtraCredits([FromBody] int count)
        {
            if (count <= 0)
                return BadRequest();

            var subscriber = await _subscriberRepository.GetByIdAsync(GetSubscriberIdFromClaims());
            subscriber!.AddCredits(count);
            await _subscriberRepository.SaveAsync(subscriber);

            return Ok(ApiResponse<object>.SuccessResult(default!, $"{count} amount of extra credits have been added."));
        }

        [NonAction]
        private Guid GetSubscriberIdFromClaims()
        {
            var subscriberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("Subscriber ID claim not found.");
            return Guid.Parse(subscriberIdClaim.Value);
        }
    }
}