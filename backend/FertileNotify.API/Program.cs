using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Application.UseCases.RegisterUser;
using FertileNotify.Application.Services;
using FertileNotify.Infrastructure.Notifications;
using FertileNotify.Infrastructure.Persistence;
using System.Text.Json.Serialization;
using FertileNotify.Infrastructure.BackgroundJobs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<ISubscriptionRepository, InMemorySubscriptionRepository>();
builder.Services.AddSingleton<ITemplateRepository, InMemoryTemplateRepository>();

builder.Services.AddSingleton<INotificationQueue, InMemoryNotificationQueue>();

builder.Services.AddScoped<INotificationSender, ConsoleNotificationSender>();
builder.Services.AddScoped<INotificationSender, EmailNotificationSender>();
builder.Services.AddScoped<INotificationSender, SMSNotificationSender>();

builder.Services.AddScoped<ProcessEventHandler>();
builder.Services.AddScoped<RegisterUserHandler>();

builder.Services.AddScoped<TemplateEngine>();

builder.Services.AddHostedService<NotificationWorker>();

var app = builder.Build();

app.MapControllers();
app.Run();
