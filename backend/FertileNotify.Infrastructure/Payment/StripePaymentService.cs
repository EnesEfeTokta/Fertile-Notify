using FertileNotify.Application.DTOs.Payments;
using Stripe;
using Microsoft.Extensions.Configuration;

namespace FertileNotify.Infrastructure.Payment
{
    public class StripePaymentService : IPaymentService
    {
        private readonly IConfiguration _config;

        public StripePaymentService(IConfiguration config)
        {
            _config = config;
            StripeConfiguration.ApiKey = config["Stripe:SecretKey"];
        }

        public async Task<string> CreateCustomerAsync(Subscriber subscriber)
        {
            var options = new CustomerCreateOptions
            {
                Email = subscriber.Email.Value,
                Name = subscriber.CompanyName.Name
            };
            var service = new CustomerService();
            var customer = await service.CreateAsync(options);
            return customer.Id;
        }

        public async Task<string> CreatePaymentIntentAsync(long amount, string customerId)
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = amount,
                Currency = "usd",
                Customer = customerId,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
            };
            var service = new PaymentIntentService();
            var intent = await service.CreateAsync(options);
            return intent.ClientSecret ?? string.Empty;
        }

        public async Task<ExtraCreditPaymentIntentDto> CreateExtraCreditPaymentIntentAsync(Subscriber subscriber, int credits)
        {
            var pricePerCreditInCents = long.TryParse(_config["Stripe:CreditPricePerUnitCents"], out var configuredPrice)
                ? configuredPrice
                : 100L;
            var currency = _config["Stripe:Currency"] ?? "usd";
            var amountInCents = credits * pricePerCreditInCents;

            var options = new PaymentIntentCreateOptions
            {
                Amount = amountInCents,
                Currency = currency,
                ReceiptEmail = subscriber.Email.Value,
                AutomaticPaymentMethods = new PaymentIntentAutomaticPaymentMethodsOptions { Enabled = true },
                Metadata = new Dictionary<string, string>
                {
                    ["subscriberId"] = subscriber.Id.ToString(),
                    ["credits"] = credits.ToString()
                }
            };

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            return new ExtraCreditPaymentIntentDto
            {
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret ?? string.Empty,
                Credits = credits,
                AmountInCents = amountInCents,
                Currency = currency
            };
        }
    }
}