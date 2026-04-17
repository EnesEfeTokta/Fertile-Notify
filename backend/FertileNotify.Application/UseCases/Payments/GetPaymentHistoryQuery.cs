namespace FertileNotify.Application.UseCases.Payments
{
    public class GetPaymentHistoryQuery : IQuery<List<PaymentLogDto>>
    {
        public Guid SubscriberId { get; set; }
    }

    public class GetPaymentHistoryHandler : IQueryHandler<GetPaymentHistoryQuery, List<PaymentLogDto>>
    {
        private readonly IPaymentLogRepository _paymentLogRepository;

        public GetPaymentHistoryHandler(IPaymentLogRepository paymentLogRepository)
        {
            _paymentLogRepository = paymentLogRepository;
        }

        public async Task<List<PaymentLogDto>> Handle(GetPaymentHistoryQuery request, CancellationToken cancellationToken)
        {
            var paymentLogs = await _paymentLogRepository.GetPaymentLogsBySubscriberIdAsync(request.SubscriberId);

            return paymentLogs.Select(paymentLog => new PaymentLogDto
            {
                Id = paymentLog.Id,
                StripePaymentIntentId = paymentLog.StripePaymentIntentId,
                Amount = paymentLog.Amount,
                Status = paymentLog.Status,
                CreatedAt = paymentLog.CreatedAt
            }).ToList();
        }
    }
}