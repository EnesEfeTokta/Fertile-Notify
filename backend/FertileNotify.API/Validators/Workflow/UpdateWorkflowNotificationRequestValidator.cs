using System.Text.RegularExpressions;
using FertileNotify.API.Models.Requests;
using FertileNotify.Domain.Events;
using FertileNotify.Domain.ValueObjects;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class UpdateWorkflowNotificationRequestValidator : AbstractValidator<UpdateWorkflowNotificationRequest>
    {
        private static readonly Regex EventTriggerPattern = new("^[a-zA-Z0-9._-]+$", RegexOptions.Compiled);

        public UpdateWorkflowNotificationRequestValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Workflow id is required.");

            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Name must be at most 100 characters.")
                .Must(v => v == null || !string.IsNullOrWhiteSpace(v)).WithMessage("Name cannot be empty when provided.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must be at most 500 characters.");

            RuleFor(x => x.EventType)
                .Must(v => v == null || IsValidEventType(v))
                .WithMessage("Invalid EventType: '{PropertyValue}'.");

            RuleFor(x => x.EventTrigger)
                .MaximumLength(100).WithMessage("EventTrigger must be at most 100 characters.")
                .Must(v => v == null || EventTriggerPattern.IsMatch(v))
                .WithMessage("EventTrigger can contain only letters, numbers, '.', '_' and '-'.");

            RuleFor(x => x.Subject)
                .MaximumLength(200).WithMessage("Subject must be at most 200 characters.")
                .Must(v => v == null || !string.IsNullOrWhiteSpace(v)).WithMessage("Subject cannot be empty when provided.");

            RuleFor(x => x.Body)
                .Must(v => v == null || !string.IsNullOrWhiteSpace(v)).WithMessage("Body cannot be empty when provided.");

            RuleFor(x => x.Channel)
                .Must(v => v == null || IsValidChannel(v)).WithMessage("Channel must be valid when provided.");

            RuleFor(x => x.To)
                .Must(v => v == null || v.Count > 0).WithMessage("To cannot be an empty array when provided.")
                .Must(v => v == null || v.Any(g => g.Recipients.Any(addr => !string.IsNullOrWhiteSpace(addr))))
                .WithMessage("At least one recipient is required when 'To' is provided.");

            RuleForEach(x => x.To).ChildRules(group =>
            {
                group.RuleFor(g => g.Channel)
                    .NotEmpty().WithMessage("Channel is required for each recipient group.")
                    .Must(IsValidChannel).WithMessage("Invalid channel: '{PropertyValue}'.");

                group.RuleFor(g => g.Recipients)
                    .NotEmpty().WithMessage("Recipients cannot be empty.")
                    .Must(r => r.All(addr => !string.IsNullOrWhiteSpace(addr)))
                    .WithMessage("Recipient addresses cannot be empty or whitespace.");
            });

            RuleFor(x => x)
                .Must(HaveSingleChannelAndMatchRequestChannel)
                .When(x => x.To != null && x.To.Count > 0)
                .WithMessage("All recipient groups must use the same channel and it must match 'Channel' when provided.");
        }

        private static bool IsValidChannel(string channel)
        {
            try { NotificationChannel.From(channel); return true; }
            catch { return false; }
        }

        private static bool IsValidEventType(string eventType)
        {
            try { EventType.From(eventType); return true; }
            catch { return false; }
        }

        private static bool HaveSingleChannelAndMatchRequestChannel(UpdateWorkflowNotificationRequest request)
        {
            if (request.To == null || request.To.Count == 0)
                return true;

            var distinctChannels = request.To
                .Select(g => g.Channel)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (distinctChannels.Count != 1)
                return false;

            if (string.IsNullOrWhiteSpace(request.Channel))
                return true;

            return string.Equals(request.Channel, distinctChannels[0], StringComparison.OrdinalIgnoreCase);
        }
    }
}
