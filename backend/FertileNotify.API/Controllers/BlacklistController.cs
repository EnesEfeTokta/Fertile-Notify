using FertileNotify.API.Models.Requests;
using FertileNotify.API.Models.Responses;
using FertileNotify.Application.DTOs;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Entities;
using FertileNotify.Domain.Exceptions;
using FertileNotify.Domain.ValueObjects;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FertileNotify.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/blacklist")]
    public class BlacklistController : ControllerBase
    {
        private readonly IBlacklistRepository _blacklistRepository;

        public BlacklistController(IBlacklistRepository blacklistRepository)
        {
            _blacklistRepository = blacklistRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var items = await _blacklistRepository.GetAllBySubscriberAsync(subscriberId);
            
            var response = items.Select(i => new BlacklistEntryDto
            {
                Id = i.Id,
                RecipientAddress = i.RecipientAddress,
                Channels = i.UnwantedChannels.Select(c => c.Name).ToList(),
                IsActive = i.IsActive,
                CreatedAt = i.CreatedAt
            });

            return Ok(ApiResponse<IEnumerable<BlacklistEntryDto>>.SuccessResult(response, "Blacklist items retrieved successfully."));
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] BanRecipientRequest request)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            
            var channels = request.Channels.Select(NotificationChannel.From).ToList();
            var entry = new ForbiddenRecipient(subscriberId, request.RecipientAddress, channels);
            
            await _blacklistRepository.AddOrUpdateAsync(entry);
            
            return CreatedAtAction(nameof(GetAll), ApiResponse<object>.SuccessResult(null!, "Recipient added to blacklist."));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBanRequest request)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var entry = await _blacklistRepository.GetByIdAsync(id);

            if (entry == null || entry.UnwantedSubscriber != subscriberId)
                throw new NotFoundException("Blacklist entry not found.");

            entry.UpdateChannels(request.Channels.Select(NotificationChannel.From).ToList());
            
            if (request.IsActive) entry.Activate();
            else entry.Deactivate();

            await _blacklistRepository.AddOrUpdateAsync(entry);

            return Ok(ApiResponse<object>.SuccessResult(null!, "Blacklist entry updated."));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var entry = await _blacklistRepository.GetByIdAsync(id);

            if (entry == null || entry.UnwantedSubscriber != subscriberId)
                throw new NotFoundException("Blacklist entry not found.");

            await _blacklistRepository.DeleteAsync(id);

            return Ok(ApiResponse<object>.SuccessResult(null!, "Blacklist entry deleted."));
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
