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
            var usebleKeys = new[] 
            { 
                "Telegram_BotToken", "Discord_WebhookUrl", "WhatsApp_TwilioSid",
                "WhatsApp_TwilioToken", "WhatsApp_TwilioFrom", "Slack_AccessToken", 
                "MSTeams_WebhookUrl", "Firebase_ServiceAccountJson",
                "WebPush_VapidPublicKey", "WebPush_VapidPrivateKey",
                "WebPush_OwnerEmail", "WebPush_Icon", "WebPush_WebUrl",
                "Webhook_Secret", "SMTP_Host", "SMTP_Port", "SMTP_Email", 
                "SMTP_Password", "SMTP_OwnerName"
            };
            return settings.Keys.All(k => usebleKeys.Contains(k));
        }
    }
}
