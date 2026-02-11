using FertileNotify.API.Models;
using FertileNotify.Domain.Enums;
using FluentValidation;
using System.Text.RegularExpressions;

namespace FertileNotify.API.Validators
{
    public class RegisterSubscriberRequestValidator : AbstractValidator<RegisterSubscriberRequest>
    {
        public RegisterSubscriberRequestValidator() 
        {
            RuleFor(x => x.CompanyName)
                .NotEmpty().WithMessage("Company name is a required field.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is a required field.")
                .Must(IsValidPassword)
                .WithMessage("Password must be at least 8 characters long and contain at least one uppercase letter, one lowercase letter, and one digit.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is a required field.")
                .EmailAddress().WithMessage("Please enter your email address in the correct format.");

            RuleFor(x => x.PhoneNumber)
                .Must(BeAValidPhoneNumber)
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Please enter a valid phone number.");

            RuleFor(x => x.Plan)
                .NotEmpty().WithMessage("Plan(Subscription Plan) is a required field.")
                .Must(SubscriptionPlanValid).WithMessage("Invalid Subscription Plan.");
        }

        private bool BeAValidPhoneNumber(string? phoneNumber)
            => Regex.IsMatch(phoneNumber!, @"^[\d\s\-\+\(\)]+$");

        private bool SubscriptionPlanValid(string plan)
            => Enum.TryParse<SubscriptionPlan>(plan, ignoreCase: true, out _);

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
