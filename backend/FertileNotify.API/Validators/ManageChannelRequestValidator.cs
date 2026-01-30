using FertileNotify.API.Models;
using FertileNotify.Domain.ValueObjects;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class ManageChannelRequestValidator : AbstractValidator<ManageChannelRequest>
    {
        public ManageChannelRequestValidator()
        {
            RuleFor(x => x.Channel)
                .NotEmpty().WithMessage("Channel is a required field.")
                .Must(ChannelValid).WithMessage("Invalid Channel type.");
        }

        private bool ChannelValid(string channel)
        {
            try { NotificationChannel.From(channel); return true; }
            catch { return false; }
        }
    }
}
