using FluentValidation;
using FertileNotify.API.Models;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.API.Validators
{
    public class GetTemplatesRequestValidator : AbstractValidator<GetTemplatesRequest>
    {
        public GetTemplatesRequestValidator() 
        {
            RuleFor(x => x.Queries)
                .NotEmpty().WithMessage("The query list cannot be empty.");

            RuleForEach(x => x.Queries).ChildRules(query =>
            {
                query.RuleFor(q => q.EventType)
                    .NotEmpty().WithMessage("The event type cannot be empty.")
                    .Must(BeAValidEventType).WithMessage("Invalid event type.");

                query.RuleFor(q => q.Channel)
                    .NotEmpty().WithMessage("The notification channel cannot be empty.")
                    .Must(BeAValidChannel).WithMessage("Invalid notification channel.");
            });
        }

        private bool BeAValidEventType(string eventType)
        {
            try { return EventType.From(eventType) != null; }
            catch { return false; }
        }

        private bool BeAValidChannel(string channel)
        {
            try { return NotificationChannel.From(channel) != null; }
            catch { return false; }
        }
    }
}
