namespace FertileNotify.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/system-notifications")]
    public class SystemNotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SystemNotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] string? status = "all")
        {
            if (!TryParseStatus(status, out var isRead, out var errorMessage))
            {
                return BadRequest(ApiResponse<object>.FailureResult(new List<string> { errorMessage! }));
            }

            var notifications = await _mediator.Send(new ListSystemNotificationsQuery
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                IsRead = isRead
            });

            var filterLabel = isRead.HasValue
                ? (isRead.Value ? "read" : "unread")
                : "all";

            return Ok(ApiResponse<IEnumerable<SystemNotificationDto>>.SuccessResult(
                notifications,
                $"{notifications.Count} {filterLabel} system notification(s) found."));
        }

        [HttpPatch("{notificationId:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            await _mediator.Send(new MarkSystemNotificationAsReadCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                NotificationId = notificationId
            });

            return Ok(ApiResponse<object>.SuccessResult(
                new { status = "read" },
                "System notification marked as read."));
        }

        [NonAction]
        private Guid GetSubscriberIdFromClaims()
        {
            var subscriberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("Subscriber ID claim not found.");
            return Guid.Parse(subscriberIdClaim.Value);
        }

        [NonAction]
        private static bool TryParseStatus(string? status, out bool? isRead, out string? errorMessage)
        {
            isRead = null;
            errorMessage = null;

            if (string.IsNullOrWhiteSpace(status) || status.Equals("all", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (status.Equals("read", StringComparison.OrdinalIgnoreCase))
            {
                isRead = true;
                return true;
            }

            if (status.Equals("unread", StringComparison.OrdinalIgnoreCase))
            {
                isRead = false;
                return true;
            }

            errorMessage = "status must be one of: all, read, unread.";
            return false;
        }
    }
}
