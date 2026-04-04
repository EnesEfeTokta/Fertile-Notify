using FertileNotify.API.Models.Requests;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class SendNotificationRequestValidator : AbstractValidator<SendNotificationRequest>
    {
        public SendNotificationRequestValidator()
        {
            RuleFor(x => x.To)
                .NotEmpty().WithMessage("Recipient list ('To') cannot be empty.")
                .Must(to => to.Sum(group => group.Recipients.Count) <= 1000)
                .WithMessage("Total recipients across all channels cannot exceed 1000 in a single request.");

            RuleForEach(x => x.To).ChildRules(group =>
            {
                group.RuleFor(g => g.Channel)
                    .NotEmpty().WithMessage("Channel name is required.")
                    .Must(ChannelValid).WithMessage("Invalid Channel: '{PropertyValue}'.");

                group.RuleFor(g => g.Recipients)
                    .NotEmpty().WithMessage("Recipient list for channel '{ParentContext.InstanceToValidate.Channel}' cannot be empty.")
                    .Must(r => r.All(addr => !string.IsNullOrWhiteSpace(addr)))
                    .WithMessage("Recipient addresses cannot be empty or whitespace.");
            });

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
        {
            if (parameters == null) return true;
            return !parameters.Any(p => string.IsNullOrWhiteSpace(p.Key) || string.IsNullOrWhiteSpace(p.Value));
        }
    }
}