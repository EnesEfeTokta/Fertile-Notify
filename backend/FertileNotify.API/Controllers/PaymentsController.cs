using Stripe;

namespace FertileNotify.API.Controllers
{
    [Authorize]
    [Route("api/payments")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(IMediator mediator, IConfiguration configuration, ILogger<PaymentsController> logger)
        {
            _mediator = mediator;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("extra-credits/intent")]
        public async Task<IActionResult> CreateExtraCreditPaymentIntent([FromBody] CreateExtraCreditPaymentIntentRequest request)
        {
            var result = await _mediator.Send(new CreateExtraCreditPaymentIntentCommand
            {
                SubscriberId = GetSubscriberIdFromClaims(),
                Credits = request.Credits
            });

            return Ok(ApiResponse<object>.SuccessResult(result, "Payment intent created successfully."));
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook()
        {
            var webhookSecret = _configuration["Stripe:WebhookSecret"];
            if (string.IsNullOrWhiteSpace(webhookSecret))
                throw new BusinessRuleException("Stripe webhook secret is not configured.", "PAY_1003");

            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            Event stripeEvent;
            try
            {
                stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], webhookSecret);
            }
            catch (StripeException ex)
            {
                _logger.LogWarning(ex, "Stripe webhook signature verification failed.");
                return BadRequest(ApiResponse<object>.FailureResult(new List<string> { "Invalid Stripe signature." }, "Webhook verification failed."));
            }

            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded && stripeEvent.Data.Object is PaymentIntent paymentIntent)
            {
                if (!TryGetWebhookPayload(paymentIntent, out var subscriberId, out var credits))
                {
                    _logger.LogWarning("Stripe webhook missing required metadata for payment intent {PaymentIntentId}.", paymentIntent.Id);
                    return Ok(ApiResponse<object>.SuccessResult(default!, "Webhook ignored due to missing metadata."));
                }

                await _mediator.Send(new ApplySuccessfulExtraCreditPaymentCommand
                {
                    SubscriberId = subscriberId,
                    Credits = credits,
                    PaymentIntentId = paymentIntent.Id
                });
            }

            return Ok(ApiResponse<object>.SuccessResult(default!, "Webhook processed."));
        }

        [NonAction]
        private Guid GetSubscriberIdFromClaims()
        {
            var subscriberIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedException("Subscriber ID claim not found.");
            return Guid.Parse(subscriberIdClaim.Value);
        }

        [NonAction]
        private static bool TryGetWebhookPayload(PaymentIntent paymentIntent, out Guid subscriberId, out int credits)
        {
            subscriberId = Guid.Empty;
            credits = 0;

            if (!paymentIntent.Metadata.TryGetValue("subscriberId", out var subscriberIdRaw))
                return false;
            if (!paymentIntent.Metadata.TryGetValue("credits", out var creditsRaw))
                return false;

            if (!Guid.TryParse(subscriberIdRaw, out subscriberId))
                return false;
            if (!int.TryParse(creditsRaw, out credits))
                return false;
            if (credits <= 0)
                return false;

            return true;
        }
    }
}
