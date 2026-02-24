using FluentValidation;
using FertileNotify.API.Models.Requests;

namespace FertileNotify.API.Validators
{
    public class CreateApiKeyRequestValidator : AbstractValidator<CreateApiKeyRequest>
    {
        public CreateApiKeyRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("API key name is required.");
        }
    }
}
