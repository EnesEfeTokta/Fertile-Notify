using FertileNotify.API.Models.Requests;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class OtpRequestValidator : AbstractValidator<OtpRequest>
    {
        public OtpRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.OtpCode)
                .NotEmpty().WithMessage("OTP code is required.")
                .Length(6).WithMessage("OTP code must be 6 characters long.")
                .Must(code => code.All(char.IsDigit)).WithMessage("OTP code must contain only digits.");
        }
    }
}
