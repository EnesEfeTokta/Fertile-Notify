using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Application.UseCases.CreateUserWithSubscription;
using FertileNotify.Infrastructure.Notifications;
using FertileNotify.Infrastructure.Persistence;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddScoped<IUserRepository, InMemoryUserRepository>();
builder.Services.AddScoped<ISubscriptionRepository, InMemorySubscriptionRepository>();
builder.Services.AddScoped<INotificationSender, ConsoleNotificationSender>();

builder.Services.AddScoped<CreateUserHandler>();
builder.Services.AddScoped<ProcessEventHandler>();

var app = builder.Build();

app.MapControllers();
app.Run();
