using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class UpdateApiKeyStatusRequestValidator : AbstractValidator<UpdateApiKeyStatusRequest>
    {
        public UpdateApiKeyStatusRequestValidator()
        {
            RuleFor(x => x.IsActive)
                .NotNull()
                .WithMessage("API key status is required.");
        }
    }
}
