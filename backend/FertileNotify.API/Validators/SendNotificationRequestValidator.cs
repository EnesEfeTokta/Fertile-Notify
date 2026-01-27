using FertileNotify.API.Models;
using FluentValidation;
using FertileNotify.Domain.Events;

namespace FertileNotify.API.Validators
{
    public class SendNotificationRequestValidator : AbstractValidator<SendNotificationRequest>
    {
        public SendNotificationRequestValidator()
        {
            RuleFor(x => x.EventType)
                .NotEmpty().WithMessage("EventType is required.")
                .Must(EventTypeValid).WithMessage("Invalid EventType.");

            RuleFor(x => x.Parameters)
                .NotNull().WithMessage("Parameters object cannot be null.")
                .Must(CheckParameters).WithMessage("Parameters contain invalid keys or values.");
        }

        private bool GuidValid(Guid id)
            => id != Guid.Empty;

        private bool EventTypeValid(string eventType)
        {
            try { EventType.From(eventType); return true; }
            catch { return false; }
        }

        private bool CheckParameters(Dictionary<string, string> parameters)
            => !parameters.Any(p => string.IsNullOrWhiteSpace(p.Key) || string.IsNullOrWhiteSpace(p.Value));
    }
}