using FertileNotify.API.Models;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator() 
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is a required field.")
                .EmailAddress().WithMessage("Please enter your email address in the correct format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is a required field.")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
        }
    }
}
