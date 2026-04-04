using FertileNotify.API.Models.Requests;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidator() 
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
