using FertileNotify.API.Models;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
    {
        public RefreshTokenRequestValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token is required.")
                .MinimumLength(20).WithMessage("Refresh token must be at least 20 characters long.");
        }
    }
}
