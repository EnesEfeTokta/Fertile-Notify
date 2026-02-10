using FertileNotify.API.Models;
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
                .NotEmpty().WithMessage("Channel is required.");

            RuleFor(x => x.SubjectTemplate)
                .NotEmpty().WithMessage("SubjectTemplate is required.");

            RuleFor(x => x.BodyTemplate)
                .NotEmpty().WithMessage("BodyTemplate is required.");
        }

        private bool BeAValidEventType(string eventType)
        {
            try
            {
                Domain.Events.EventType.From(eventType);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
