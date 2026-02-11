using FertileNotify.API.Models;
using FertileNotify.Domain.ValueObjects;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class CreateTemplateRequestValidator : AbstractValidator<CreateTemplateRequest>
    {
        public CreateTemplateRequestValidator()
        {
            RuleFor(x => x.EventType)
                .NotEmpty().WithMessage("EventType is required.")
                .Must(BeAValidEventType).WithMessage("Invalid EventType.");

            RuleFor(x => x.Channel)
                .NotEmpty().WithMessage("Channel is a required field.")
                .Must(ChannelValid).WithMessage("Invalid Channel type.");

            RuleFor(x => x.SubjectTemplate)
                .NotEmpty().WithMessage("SubjectTemplate is required.");

            RuleFor(x => x.BodyTemplate)
                .NotEmpty().WithMessage("BodyTemplate is required.");
        }

        private bool BeAValidEventType(string eventType)
        {
            try { Domain.Events.EventType.From(eventType); return true; }
            catch { return false; }
        }
        private bool ChannelValid(string channel)
        {
            try { NotificationChannel.From(channel); return true; }
            catch { return false; }
        }
    }
}
