using FertileNotify.API.Models;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class SendBulkNotificationRequestValidator : AbstractValidator<SendBulkNotificationRequest>
    {
        public SendBulkNotificationRequestValidator()
        {
            RuleFor(x => x.Channel)
                .NotEmpty().WithMessage("Channel is required.")
                .Must(ChannelValid).WithMessage("Invalid channel.");

            RuleFor(x => x.Recipients)
                .NotEmpty().WithMessage("Recipient list cannot be empty.")
                .Must(list => list.Count <= 1000).WithMessage("Max 1000 recipients allowed.");

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

        private bool ChannelValid(string channel)
        {
            try { NotificationChannel.From(channel); return true; }
            catch { return false; }
        }

        private bool CheckParameters(Dictionary<string, string> parameters)
            => !parameters.Any(p => string.IsNullOrWhiteSpace(p.Key) || string.IsNullOrWhiteSpace(p.Value));
    }
}
