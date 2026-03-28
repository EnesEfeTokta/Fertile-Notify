using FertileNotify.Infrastructure.BackgroundJobs;
using FertileNotify.Application.Interfaces;
using FertileNotify.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace FertileNotify.API.Extensions
{
    public static class InfrastructureExtension
    {
        public static IServiceCollection AddInfrastructureConfig(this IServiceCollection services, IConfiguration configuration, IHostEnvironment environment)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // Health Checks
            var healthChecks = services.AddHealthChecks();
            if (!environment.IsEnvironment("Testing"))
            {
                healthChecks.AddNpgSql(configuration.GetConnectionString("DefaultConnection")!);
            }

            if (environment.IsEnvironment("Testing"))
            {
                services.AddDistributedMemoryCache();
                return services;
            }

            // Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["Redis:ConnectionString"] ?? "localhost:6379";
                options.InstanceName = "FertileNotify_";
            });
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"] ?? "localhost:6379,abortConnect=false"));
            services.AddScoped<RedisSchedulerService>();
            services.AddScoped<IWorkflowScheduleService, RedisWorkflowScheduleService>();

            // MassTransit (RabbitMQ)
            services.AddMassTransit(x =>
            {
                x.AddConsumer<NotificationConsumer>();

                x.UsingRabbitMq((context, cfg) =>
                {
                    cfg.Host(configuration["RabbitMQ:Host"], "/", h =>
                    {
                        h.Username(configuration["RabbitMQ:Username"] ?? "guest");
                        h.Password(configuration["RabbitMQ:Password"] ?? "guest");
                    });

                    cfg.ReceiveEndpoint("notification-queue", e =>
                    {
                        e.PrefetchCount = 16;
                        e.EnablePriority(10);
                        e.UseMessageRetry(r => r.Intervals(
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(15),
                            TimeSpan.FromSeconds(30)));

                        e.ConfigureConsumer<NotificationConsumer>(context);
                    });
                });
            });

            // Background Workers
            services.AddHostedService<LogRetentionWorker>();
            services.AddHostedService<AutomationWorker>();

            return services;
        }
    }
}
