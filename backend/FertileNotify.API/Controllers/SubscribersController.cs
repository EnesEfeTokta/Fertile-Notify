using FertileNotify.Application.UseCases.UpdateSubscriberProfile;

namespace FertileNotify.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/subscribers")]
    public class SubscriberController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ISubscriberRepository _subscriberRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IApiKeyRepository _apiKeyRepository;
        private readonly ISubscriberChannelRepository _subscriberChannelRepository;

        public SubscriberController(
            IMediator mediator,
            ISubscriptionRepository subscriptionRepository,
            ISubscriberRepository subscriberRepository,
            IApiKeyRepository apiKeyRepository,
            ISubscriberChannelRepository subscriberChannelRepository)
        {
            _mediator = mediator;
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
                CompanyDescription = subscriber.CompanyDescription,
                LogoUrl = subscriber.LogoUrl?.Value,
                WebsiteUrl = subscriber.WebsiteUrl?.Value,
                Location = subscriber.Location,
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

            return Ok(ApiResponse<SubscriberDto>.SuccessResult(response,
                "Subscription information belonging to the subscriber."));
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateSubscriberProfileRequest request)
        {
            await _mediator.Send(new UpdateSubscriberProfileCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                CompanyName = request.CompanyName,
                CompanyDescription = request.CompanyDescription,
                LogoUrl = request.LogoUrl,
                WebsiteUrl = request.WebsiteUrl,
                Location = request.Location,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
            });
            return Ok(ApiResponse<object>.SuccessResult(default!, "Profile updated successfully."));
        }

        [HttpPut("password")]
        public async Task<IActionResult> UpdatePassword([FromBody] UpdatePasswordRequest request)
        {
            await _mediator.Send(new UpdatePasswordCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            });
            return Ok(ApiResponse<object>.SuccessResult(default!,
                "The subscriber's password has been updated."));
        }

        [HttpPost("channels")]
        public async Task<IActionResult> UpdateChannels([FromBody] ManageChannelRequest request)
        {
            await _mediator.Send(new ManageChannelsCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                Channel = request.Channel,
                Enable = request.Enable
            });
            return Ok(ApiResponse<object>.SuccessResult(default!,
                "The subscriber's channel settings have been updated."));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterSubscriberRequest request)
        {
            Enum.TryParse<SubscriptionPlan>(request.Plan, ignoreCase: true, out var plan);
            await _mediator.Send(new RegisterSubscriberCommand
            {
                CompanyName = CompanyName.Create(request.CompanyName),
                CompanyDescription = request.CompanyDescription,
                LogoUrl = string.IsNullOrWhiteSpace(request.LogoUrl)
                        ? null : CustomUrl.Create(request.LogoUrl),
                WebsiteUrl = string.IsNullOrWhiteSpace(request.WebsiteUrl)
                        ? null : CustomUrl.Create(request.WebsiteUrl),
                Location = request.Location,
                Password = Password.Create(request.Password),
                Email = EmailAddress.Create(request.Email),
                PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                    ? null : PhoneNumber.Create(request.PhoneNumber),
                Plan = plan,
            });
            return Ok(ApiResponse<RegisterSubscriberCommand>.SuccessResult(default!,
                "Registration successful, log in."));
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            await _mediator.Send(new DeleteAccountCommand
            {
                SubscriberId = GetSubscriberIdFromClaims()
            });
            return Ok(ApiResponse<object>.SuccessResult(default!,
                "The subscriber's account has been deleted."));
        }

        [HttpPost("api-keys")]
        public async Task<IActionResult> CreateApiKey([FromBody] CreateApiKeyRequest request)
        {
            var rawApiKey = await _mediator.Send(new CreateApiKeyCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                Name = request.Name
            });
            return Ok(ApiResponse<object>.SuccessResult(
                new { ApiKey = rawApiKey },
                "Please save this key securely. You won't be able to see it again."));
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
                Scopes = k.Scopes,
                IsActive = k.IsActive,
                CreatedAt = k.CreatedAt,
            });
            return Ok(ApiResponse<IEnumerable<ApiKeyDto>>.SuccessResult(response,
                "API keys retrieved."));
        }

        [HttpPatch("api-keys/{apiKeyId}/scopes")]
        public async Task<IActionResult> UpdateApiKeyScopes(Guid apiKeyId, [FromBody] UpdateApiKeyScopesRequest request)
        {
            await _mediator.Send(new UpdateApiKeyScopesCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                ApiKeyId = apiKeyId,
                Scopes = request.Scopes
            });

            return Ok(ApiResponse<object>.SuccessResult(default!,
                "The API key scopes have been updated."));
        }

        [HttpPatch("api-keys/{apiKeyId}/status")]
        public async Task<IActionResult> UpdateApiKeyStatus(Guid apiKeyId, [FromBody] UpdateApiKeyStatusRequest request)
        {
            await _mediator.Send(new UpdateApiKeyStatusCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                ApiKeyId = apiKeyId,
                IsActive = request.IsActive
            });

            return Ok(ApiResponse<object>.SuccessResult(default!,
                "The API key status has been updated."));
        }

        [HttpDelete("api-keys/{apiKeyId}")]
        public async Task<IActionResult> DeleteApiKey(Guid apiKeyId)
        {
            await _mediator.Send(new DeleteApiKeyCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                ApiKeyId = apiKeyId
            });
            return Ok(ApiResponse<object>.SuccessResult(default!,
                "The API key has been deleted."));
        }

        [HttpPost("settings/channel-setting")]
        public async Task<IActionResult> SetChannelSetting([FromBody] ChannelSettingRequest request)
        {
            await _mediator.Send(new SetChannelSettingCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                Channel = request.Channel,
                Settings = request.Settings
            });
            return Ok(ApiResponse<object>.SuccessResult(default!,
                $"{request.Channel} configured successfully."));
        }

        [HttpGet("settings/channel-setting")]
        public async Task<IActionResult> GetChannelSetting([FromQuery] string channel)
        {
            var setting = await _subscriberChannelRepository.GetSettingAsync(
                GetSubscriberIdFromClaims(),
                NotificationChannel.From(channel));

            if (setting == null)
                return NotFound(new { message = $"No settings found for {channel}." });

            return Ok(ApiResponse<SubscriberChannelSetting>.SuccessResult(setting,
                $"{channel} settings retrieved successfully."));
        }

        [HttpPatch("add-extra-credits")]
        public async Task<IActionResult> AddExtraCredits([FromBody] int count)
        {
            if (count <= 0) return BadRequest();

            var subscriber = await _subscriberRepository.GetByIdAsync(GetSubscriberIdFromClaims());
            subscriber!.AddCredits(count);
            await _subscriberRepository.SaveAsync(subscriber);

            return Ok(ApiResponse<object>.SuccessResult(default!,
                $"{count} extra credits have been added."));
        }

        [HttpGet("export-data")]
        public async Task<IActionResult> ExportData()
        {
            var exportData = await _mediator.Send(new ExportDataQuery
            {
                SubscriberId = GetSubscriberIdFromClaims()
            });

            var json  = JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
            var bytes = Encoding.UTF8.GetBytes(json);
            var file  = $"fertilenotify-export-{DateTime.UtcNow:yyyyMMdd-HHmmss}.json";

            return File(bytes, "application/json", file);
        }

        [NonAction]
        private Guid GetSubscriberIdFromClaims()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("Subscriber ID claim not found.");
            return Guid.Parse(claim.Value);
        }
    }
}