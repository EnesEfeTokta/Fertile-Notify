using FluentValidation;
using FertileNotify.API.Models;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;

namespace FertileNotify.API.Validators
{
    public class GetCustomTemplatesRequestValidator : AbstractValidator<GetCustomTemplatesRequest>
    {
        public GetCustomTemplatesRequestValidator() 
        {
            RuleFor(x => x.Queries)
                .NotEmpty().WithMessage("Empty queries are not allowed.")
                .Must(QueryParametersValid).WithMessage("Query parameters cannot be empty or meaningless.");
        }

        private bool QueryParametersValid(GetCustomTemplateQuery[] queries)
        {
            foreach (var query in queries)
            {
                if (string.IsNullOrEmpty(query.EventType))
                {
                    if (EventType.From(query.EventType) == null) return false;
                }

                if (string.IsNullOrEmpty(query.Channel))
                {
                    if (NotificationChannel.From(query.Channel) == null) return false;
                }
            }
            return true;
        }
    }
}
