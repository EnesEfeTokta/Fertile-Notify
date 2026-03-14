using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Security;
using FertileNotify.Application.Services;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Application.UseCases.RegisterSubscriber;
using FertileNotify.Infrastructure.Notifications;
using FertileNotify.Infrastructure.Persistence;
using Mjml.Net;

namespace FertileNotify.API.Extensions;

public static class ApplicationServiceExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<ISubscriberRepository, EfSubscriberRepository>();
        services.AddScoped<ISubscriptionRepository, EfSubscriptionRepository>();
        services.AddScoped<ITemplateRepository, EfTemplateRepository>();
        services.AddScoped<IApiKeyRepository, EfApiKeyRepository>();
        services.AddScoped<INotificationLogRepository, EfNotificationLogRepository>();
        services.AddScoped<ISubscriberChannelRepository, EfSubscriberChannelRepository>();
        services.AddScoped<IStatsRepository, EfStatsRepository>();
        services.AddScoped<IBlacklistRepository, EfBlacklistRepository>();

        // Application Services
        services.AddScoped<IStatisticsService, StatisticsService>();
        services.AddScoped<IEmailService, SmtpEmailService>();
        services.AddScoped<ISecurityService, SecurityService>();
        services.AddScoped<TemplateEngine>();
        services.AddSingleton<IMjmlRenderer, MjmlRenderer>();
        services.AddHttpClient();

        // Use Cases
        services.AddScoped<ProcessEventHandler>();
        services.AddScoped<RegisterSubscriberHandler>();

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
