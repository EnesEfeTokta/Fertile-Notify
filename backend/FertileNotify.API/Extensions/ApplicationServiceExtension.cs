using FertileNotify.Application;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Security;
using FertileNotify.Application.Services;
using FertileNotify.Infrastructure.Notifications;
using FertileNotify.Infrastructure.Persistence;
using FertileNotify.Infrastructure.BackgroundJobs;

using Mjml.Net;

namespace FertileNotify.API.Extensions
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddApplication();

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
            services.AddScoped<IAutomationRepository, EfAutomationRepository>();

            // Application Services
            services.AddScoped<IStatisticsService, StatisticsService>();
            services.AddScoped<INotificationLogService, RedisNotificationLogService>();
            services.AddScoped<INotificationDispatchService, NotificationDispatchService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<TemplateEngine>();
            services.AddScoped<AutomationTriggerService>();
            services.AddScoped<AutomationSchedulerService>();
            services.AddSingleton<IMjmlRenderer, MjmlRenderer>();
            services.AddHttpClient();

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
