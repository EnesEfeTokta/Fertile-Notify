using FertileNotify.API.Models.Requests;
using FertileNotify.Domain.Enums;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class ComplaintRequestValidator : AbstractValidator<ComplaintRequest>
    {
        public ComplaintRequestValidator() 
        {
            RuleFor(x => x.SubscriberId)
                .NotEmpty().WithMessage("SubscriberId is required.");

            RuleFor(x => x.ReporterEmail)
                .NotEmpty().WithMessage("ReporterEmail is required.")
                .EmailAddress().WithMessage("ReporterEmail must be a valid email address.");

            RuleFor(x => x.Reason)
                .NotEmpty().WithMessage("Reason is required.")
                .Must(ReasonValid).WithMessage("Invalid Reason Type.");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");

            RuleFor(x => x.NotificationSubject)
                .NotEmpty().WithMessage("NotificationSubject is required.");

            RuleFor(x => x.NotificationBody)
                .NotEmpty().WithMessage("NotificationBody is required.");
        }

        private bool ReasonValid(string reason)
            => Enum.TryParse<ComplaintType>(reason, ignoreCase: true, out _);
    }
}
