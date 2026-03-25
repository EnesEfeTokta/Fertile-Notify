using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Security;
using FertileNotify.Application.Services;
using FertileNotify.Application.UseCases.RegisterSubscriber;
using FertileNotify.Application.UseCases.SendNotification;
using FertileNotify.Application.UseCases.Unsubscribe;
using FertileNotify.Application.UseCases.UpdateContactInfo;
using FertileNotify.Application.UseCases.UpdateCompanyName;
using FertileNotify.Application.UseCases.ManageChannels;
using FertileNotify.Application.UseCases.UpdatePassword;
using FertileNotify.Application.UseCases.CreateApiKey;
using FertileNotify.Application.UseCases.RevokeApiKey;
using FertileNotify.Application.UseCases.SetChannelSetting;
using FertileNotify.Application.UseCases.Login;
using FertileNotify.Application.UseCases.VerifyCode;
using FertileNotify.Application.UseCases.ForgotPassword;
using FertileNotify.Application.UseCases.RefreshToken;
using FertileNotify.Application.UseCases.DeleteAccount;
using FertileNotify.Infrastructure.Notifications;
using FertileNotify.Infrastructure.Persistence;
using FertileNotify.Application.UseCases.NotificationComplaint;

using Mjml.Net;

namespace FertileNotify.API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<ISubscriberRepository, EfSubscriberRepository>();
            services.AddScoped<ISubscriptionRepository, EfSubscriptionRepository>();
            services.Decorate<ISubscriptionRepository, CachedSubscriptionRepository>();
            services.AddScoped<ITemplateRepository, EfTemplateRepository>();
            services.AddScoped<IApiKeyRepository, EfApiKeyRepository>();
            services.AddScoped<INotificationLogRepository, EfNotificationLogRepository>();
            services.AddScoped<ISubscriberChannelRepository, EfSubscriberChannelRepository>();
            services.AddScoped<IStatsRepository, EfStatsRepository>();
            services.AddScoped<IBlacklistRepository, EfBlacklistRepository>();
            services.AddScoped<INotificationComplaintRepository, EfINotificationComplaintRepository>();

            // Application Services
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<INotificationLogService, NotificationLogService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<TemplateEngine>();
            services.AddSingleton<IMjmlRenderer, MjmlRenderer>();
            services.AddHttpClient();

            // Use Cases
            services.AddScoped<LoginHandler>();
            services.AddScoped<VerifyCodeHandler>();
            services.AddScoped<ForgotPasswordHandler>();
            services.AddScoped<RefreshTokenHandler>();
            services.AddScoped<SendNotificationHandler>();
            services.AddScoped<RegisterSubscriberHandler>();
            services.AddScoped<UnsubscribeHandler>();
            services.AddScoped<UpdateContactInfoHandler>();
            services.AddScoped<UpdateCompanyNameHandler>();
            services.AddScoped<ManageChannelsHandler>();
            services.AddScoped<UpdatePasswordHandler>();
            services.AddScoped<CreateApiKeyHandler>();
            services.AddScoped<RevokeApiKeyHandler>();
            services.AddScoped<SetChannelSettingHandler>();
            services.AddScoped<NotificationComplaintHandler>();
            services.AddScoped<DeleteAccountHandler>();

            // Notification Senders
            services.AddNotificationSenders();

            return services;
        }

        private static void AddNotificationSenders(this IServiceCollection services)
        {
            services.AddScoped<INotificationSender, ConsoleNotificationSender>();
            services.AddScoped<INotificationSender, EmailNotificationSender>();
            services.AddScoped<INotificationSender, SMSNotificationSender>();
            services.AddScoped<INotificationSender, SlackNotificationSender>();
            services.AddScoped<INotificationSender, FirebasePushNotificationSender>();
            services.AddScoped<INotificationSender, WebPushNotificationSender>();
            services.AddScoped<INotificationSender, TelegramNotificationSender>();
            services.AddScoped<INotificationSender, DiscordNotificationSender>();
            services.AddScoped<INotificationSender, WhatsAppNotificationSender>();
            services.AddScoped<INotificationSender, MSTeamsNotificationSender>();
            services.AddScoped<INotificationSender, WebhookNotificationSender>();
        }
    }
}
