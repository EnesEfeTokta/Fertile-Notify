namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/logs")]
    [Authorize]
    public class LogController : ControllerBase
    {
        private readonly INotificationLogRepository _logRepository;

        public LogController(INotificationLogRepository logRepository)
        {
            _logRepository = logRepository;
        }

        [HttpGet("{limit}")]
        public async Task<IActionResult> GetLogs(int limit = 10)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var logs = await _logRepository.GetLatestBySubscriberIdAsync(subscriberId, limit);

            var result = logs.Select(t => new
            {
                t.Id,
                t.Recipient,
                t.Channel,
                t.EventType,
                t.Content.Subject,
                t.Content.Body,
                t.Status,
                t.ErrorMessage,
                t.CreatedAt
            });

            return Ok(ApiResponse<object>.SuccessResult(result, "Notification logs retrieved successfully."));
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
