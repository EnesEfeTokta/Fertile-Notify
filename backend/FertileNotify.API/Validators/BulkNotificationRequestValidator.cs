using FertileNotify.API.Models;
using FertileNotify.Domain.Events;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class BulkNotificationRequestValidator : AbstractValidator<BulkNotificationRequest>
    {
        public BulkNotificationRequestValidator()
        {
            RuleFor(x => x.UserIds)
                .NotEmpty().WithMessage("UserIds cannot be empty.")
                .Must(ids => ids.All(id => id != Guid.Empty)).WithMessage("UserIds must contain valid GUIDs.")
                .Must(ids => ids.Distinct().Count() == ids.Count).WithMessage("UserIds must be unique.")
                .Must(ids => ids.Count <= 1000).WithMessage("UserIds cannot contain more than 1000 IDs.");

            RuleFor(x => x.EventType)
                .NotEmpty().WithMessage("EventType is required.")
                .Must(EventTypeValid).WithMessage("Invalid EventType.");

            RuleFor(x => x.Parameters)
                .NotNull().WithMessage("Parameters object cannot be null.")
                .Must(CheckParameters).WithMessage("Parameters contain invalid keys or values.");
        }

        private bool EventTypeValid(string eventType)
        {
            try { EventType.From(eventType); return true; }
            catch { return false; }
        }

        private bool CheckParameters(Dictionary<string, string> parameters)
            => !parameters.Any(p => string.IsNullOrWhiteSpace(p.Key) || string.IsNullOrWhiteSpace(p.Value));
    }
}
