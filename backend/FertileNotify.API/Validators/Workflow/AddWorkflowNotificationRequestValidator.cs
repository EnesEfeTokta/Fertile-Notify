using System.Text.RegularExpressions;
using FertileNotify.Domain.Events;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class AddWorkflowNotificationRequestValidator : AbstractValidator<AddWorkflowNotificationRequest>
    {
        private static readonly Regex EventTriggerPattern = new("^[a-zA-Z0-9._-]+$", RegexOptions.Compiled);

        public AddWorkflowNotificationRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must be at most 100 characters.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must be at most 500 characters.");

            RuleFor(x => x.EventType)
                .NotEmpty().WithMessage("EventType is required.")
                .Must(IsValidEventType).WithMessage("Invalid EventType: '{PropertyValue}'.");

            RuleFor(x => x.EventTrigger)
                .NotEmpty().WithMessage("EventTrigger is required.")
                .MaximumLength(100).WithMessage("EventTrigger must be at most 100 characters.")
                .Must(v => EventTriggerPattern.IsMatch(v)).WithMessage("EventTrigger can contain only letters, numbers, '.', '_' and '-'.");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required.")
                .MaximumLength(200).WithMessage("Subject must be at most 200 characters.");

            RuleFor(x => x.Body)
                .NotEmpty().WithMessage("Body is required.");

            RuleFor(x => x.To)
                .NotEmpty().WithMessage("Recipient list ('To') cannot be empty.")
                .Must(HaveRecipients).WithMessage("At least one recipient is required.");

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

            RuleFor(x => x.To)
                .Must(HaveUniqueChannels)
                .WithMessage("Each channel can appear only once in 'To'.");
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

        private static bool HaveRecipients(List<ChannelRecipientGroup> groups)
            => groups.Any(g => g.Recipients.Any(addr => !string.IsNullOrWhiteSpace(addr)));

        private static bool HaveUniqueChannels(List<ChannelRecipientGroup> groups)
        {
            var distinctChannels = groups
                .Select(g => g.Channel)
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            return distinctChannels.Count == groups.Count;
        }
    }
}
