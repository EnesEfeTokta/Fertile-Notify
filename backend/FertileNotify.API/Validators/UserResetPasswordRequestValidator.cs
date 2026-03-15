using FertileNotify.API.Models.Requests;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class UserResetPasswordRequestValidator : AbstractValidator<UserResetPasswordRequest>
    {
        public UserResetPasswordRequestValidator() 
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("Reset OTP Code is required.");
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Password is a required field.")
                .Must(IsValidPassword)
                .WithMessage("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit.");
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
