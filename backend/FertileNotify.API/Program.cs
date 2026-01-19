using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Application.UseCases.RegisterUser;
using FertileNotify.Domain.Enums;
using FertileNotify.Infrastructure.Notifications;
using FertileNotify.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<INotificationSender, ConsoleNotificationSender>();
builder.Services.AddScoped<ISubscriptionRepository, InMemorySubscriptionRepository>();

builder.Services.AddScoped<ProcessEventHandler>();

// === TEST ===
var userRepo = new InMemoryUserRepository();
var subscriptionRepo = new InMemorySubscriptionRepository();
var notificationSender = new ConsoleNotificationSender();

var registerUserHandler = new RegisterUserHandler(userRepo, subscriptionRepo);
var userId = await registerUserHandler.HandleAsync("test@test.com");

Console.WriteLine($"Test User ID: {userId}");

var processEvent = new ProcessEventHandler(
    subscriptionRepo,
    notificationSender
);

await processEvent.HandleAsync(new ProcessEventCommand
{
    UserId = userId,
    EventType = "OrderCreated",
    Payload = "Order #123 created"
});
// === END ===

var app = builder.Build();

app.MapControllers();
app.Run();
