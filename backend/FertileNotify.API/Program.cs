using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Infrastructure.Notifications;
using FertileNotify.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<INotificationRepository, ConsoleNotificationSender>();
builder.Services.AddScoped<ISubscriptionRepository, InMemorySubscriptionRepository>();

builder.Services.AddScoped<ProcessEventHandler>();

var app = builder.Build();

app.MapControllers();
app.Run();
