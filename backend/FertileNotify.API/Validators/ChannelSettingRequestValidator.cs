using FertileNotify.API.Models.Requests;
using FertileNotify.Domain.ValueObjects;
using FluentValidation;

namespace FertileNotify.API.Validators
{
    public class ChannelSettingRequestValidator : AbstractValidator<ChannelSettingRequest>
    {
        public ChannelSettingRequestValidator() 
        {
            RuleFor(x => x.Channel)
                .NotEmpty().WithMessage("Channel is a required field.")
                .Must(ChannelValid).WithMessage("Invalid Channel type.");

            RuleFor(x => x.Settings)
                .NotEmpty().WithMessage("Settings are required.")
                .Must(SettingsValid).WithMessage("Invalid settings.");
        }

        private bool ChannelValid(string channel)
        {
            try { NotificationChannel.From(channel); return true; }
            catch { return false; }
        }

        private bool SettingsValid(Dictionary<string, string> settings)
        {
            var usebleKeys = new[] { "TelegramBotToken", "DiscordWebhookUrl" };
            return settings.Keys.All(k => usebleKeys.Contains(k));
        }
    }
}
