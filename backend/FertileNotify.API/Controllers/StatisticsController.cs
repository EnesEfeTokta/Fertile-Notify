using FertileNotify.API.Models.Responses;
using FertileNotify.Application.Interfaces;
using FertileNotify.Domain.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FertileNotify.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/statistics")]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public StatisticsController(IStatisticsService statisticsService, ISubscriptionRepository subscriptionRepository)
        {
            _statisticsService = statisticsService;
            _subscriptionRepository = subscriptionRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetStats([FromQuery] string period = "daily")
        {
            var subscriberId = GetSubscriberIdFromClaims();

            var subscription = await _subscriptionRepository.GetBySubscriberIdAsync(subscriberId)
                ?? throw new NotFoundException("Subscription not found.");

            var stats = await _statisticsService.GetSubscriberStatsAsync(subscriberId, period.ToLower(), subscription.Plan);

            return Ok(ApiResponse<object>.SuccessResult(stats, "Statistics retrieved successfully."));
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