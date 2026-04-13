using FertileNotify.API.Models.Requests;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class CreateExtraCreditPaymentIntentRequestValidator : AbstractValidator<CreateExtraCreditPaymentIntentRequest>
    {
        public CreateExtraCreditPaymentIntentRequestValidator()
        {
            RuleFor(x => x.Credits)
                .GreaterThan(0).WithMessage("Credits must be greater than zero.")
                .LessThanOrEqualTo(1_000_000).WithMessage("Credits is too large.");
        }
    }
}
