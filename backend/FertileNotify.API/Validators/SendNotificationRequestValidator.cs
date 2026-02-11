using FertileNotify.API.Models;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class SendNotificationRequestValidator : AbstractValidator<SendNotificationRequest>
    {
        public SendNotificationRequestValidator()
        {
            RuleFor(x => x.Channel)
                .NotEmpty().WithMessage("Channel is required.")
                .Must(ChannelValid).WithMessage("Invalid Channel.");

            RuleFor(x => x.Recipient)
                .NotEmpty().WithMessage("Recipient is required.");

            RuleFor(x => x.EventType)
                .NotEmpty().WithMessage("EventType is required.")
                .Must(EventTypeValid).WithMessage("Invalid EventType.");

            RuleFor(x => x.Parameters)
                .NotNull().WithMessage("Parameters object cannot be null.")
                .Must(CheckParameters).WithMessage("Parameters contain invalid keys or values.");
        }

        private bool ChannelValid(string channel)
        {
            try { NotificationChannel.From(channel); return true; }
            catch { return false; }
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