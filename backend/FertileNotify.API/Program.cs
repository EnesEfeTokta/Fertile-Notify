using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

using FertileNotify.Application.Interfaces;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Application.UseCases.RegisterUser;
using FertileNotify.Application.Services;
using FertileNotify.Infrastructure.Notifications;
using FertileNotify.Infrastructure.Persistence;
using FertileNotify.Infrastructure.BackgroundJobs;
using FertileNotify.Infrastructure.Authentication;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Controller ve JSON Settings ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// --- 2. Database ve EF Core ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// --- 3. Repositories ---
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<ISubscriptionRepository, EfSubscriptionRepository>();
builder.Services.AddScoped<ITemplateRepository, EfTemplateRepository>();

// --- 4. Queue ve Worker ---
builder.Services.AddSingleton<INotificationQueue, InMemoryNotificationQueue>();
builder.Services.AddHostedService<NotificationWorker>();

// --- 5. Validasyon ---
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// --- 6. Auth & Token ---
builder.Services.AddScoped<ITokenService, JwtTokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("JwtSettings");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
    };
});

// --- 7. Sender and Services ---
builder.Services.AddScoped<INotificationSender, ConsoleNotificationSender>();
builder.Services.AddScoped<INotificationSender, EmailNotificationSender>();
builder.Services.AddScoped<INotificationSender, SMSNotificationSender>();

builder.Services.AddScoped<ProcessEventHandler>();
builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<TemplateEngine>();


var app = builder.Build();

// --- 8. Database Seed ---
await DbSeeder.SeedAsync(app);

app.UseMiddleware<FertileNotify.API.Middlewares.ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();