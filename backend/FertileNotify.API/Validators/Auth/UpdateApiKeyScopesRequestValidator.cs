using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class UpdateApiKeyScopesRequestValidator : AbstractValidator<UpdateApiKeyScopesRequest>
    {
        public UpdateApiKeyScopesRequestValidator()
        {
            RuleFor(x => x.Scopes)
                .MaximumLength(500)
                .WithMessage("Scopes cannot exceed 500 characters.");
        }
    }
}
