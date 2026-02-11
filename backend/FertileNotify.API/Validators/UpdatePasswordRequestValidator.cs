using FertileNotify.API.Models;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class UpdatePasswordRequestValidator : AbstractValidator<UpdatePasswordRequest>
    {
        public UpdatePasswordRequestValidator() 
        {
            RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithMessage("Current password is required.")
                .MinimumLength(8).WithMessage("Current password must be at least 8 characters long.");

            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("New password is required.")
                .Must(IsValidPassword)
                .WithMessage("New password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit.");
        }

        private bool IsValidPassword(string password)
        {
            if (string.IsNullOrEmpty(password) || 
                password.Length < 8 || 
                !password.Any(char.IsUpper) || 
                !password.Any(char.IsLower) || 
                !password.Any(char.IsDigit)
            )
                return false;
            return true;
        }
    }
}
