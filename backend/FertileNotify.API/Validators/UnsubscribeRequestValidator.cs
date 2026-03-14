using FertileNotify.API.Models.Requests;
using FertileNotify.Domain.ValueObjects;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class UnsubscribeRequestValidator : AbstractValidator<UnsubscribeRequest>
    {
        public UnsubscribeRequestValidator() 
        {
            RuleFor(x => x.Recipient)
                .NotEmpty().WithMessage("Recipient is required.");
            RuleFor(x => x.SubscriberId)
                .NotEmpty().WithMessage("SubscriberId is required.");
            RuleFor(x => x.Token)
                .NotEmpty().WithMessage("Token is required.");
            RuleFor(x => x.Channels)
                .Must(channels => channels == null || channels.All(IsValidChannel))
                .WithMessage("One or more channels are invalid.");
        }

        private bool IsValidChannel(string channel)
        {
            try { NotificationChannel.From(channel); return true; }
            catch { return false; }
        }
    }
}
