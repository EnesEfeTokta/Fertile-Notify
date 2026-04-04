using FertileNotify.API.Models.Requests;
using FluentValidation;
using System.Text.RegularExpressions;

namespace FertileNotify.API.Validators
{
    public class UpdateContactRequestValidator : AbstractValidator<UpdateContactRequest>
    {
        public UpdateContactRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is a required field.")
                .EmailAddress().WithMessage("Please enter your email address in the correct format.");

            RuleFor(x => x.PhoneNumber)
                .Must(BeAValidPhoneNumber)
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Please enter a valid phone number.");
        }

        private bool BeAValidPhoneNumber(string? phoneNumber)
            => Regex.IsMatch(phoneNumber!, @"^[\d\s\-\+\(\)]+$");
    }
}
