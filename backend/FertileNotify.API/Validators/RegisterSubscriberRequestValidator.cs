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
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one digit.")
                .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain at least one special character.");

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
    }
}
