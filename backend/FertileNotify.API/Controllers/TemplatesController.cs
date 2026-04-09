namespace FertileNotify.API.Controllers
{
    [ApiController]
    [Route("api/templates")]
    [Authorize]
    public class TemplatesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TemplatesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllTemplates()
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var result = await _mediator.Send(new GetAllTemplatesQuery
            {
                SubscriberId = subscriberId
            });

            return Ok(ApiResponse<object>.SuccessResult(result, "Templates retrieved successfully."));
        }

        [HttpPost("query")]
        public async Task<IActionResult> GetTemplates([FromBody] GetTemplatesRequest request)
        {
            var subscriberId = GetSubscriberIdFromClaims();
            var result = await _mediator.Send(new GetTemplatesQuery
            {
                SubscriberId = subscriberId,
                IsTemplateTypeCustom = request.IsTemplateTypeCustom,
                Queries = request.Queries.Select(q => new TemplateQueryItem
                {
                    EventType = q.EventType,
                    Channel = q.Channel
                }).ToList()
            });

            return Ok(ApiResponse<object>.SuccessResult(result, "Queried templates retrieved successfully."));
        }

        [HttpPost("create-or-update-custom")]
        public async Task<IActionResult> CreateOrUpdateCustom([FromBody] CreateTemplateRequest request)
        {
            await _mediator.Send(new CreateOrUpdateCustomTemplateCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                Name = request.Name,
                Description = request.Description,
                EventType = request.EventType,
                Channel = request.Channel,
                Subject = request.Subject,
                Body = request.Body
            });

            return Ok(ApiResponse<object>.SuccessResult(default!, "Template created or updated successfully."));
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
