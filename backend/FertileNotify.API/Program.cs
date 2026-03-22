using FertileNotify.API.Extensions;
using FertileNotify.API.Middlewares;
using FertileNotify.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Serilog;

// --- INITIALIZATION ---
var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURATION ---
builder.Configuration.AddCustomEnvVariables(); // Custom extension

// --- LOGGING ---
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();

// --- SERVICES REGISTRATION ---
builder.Services
    .AddWebConfig()             // CORS, Rate Limiting, Controllers, Validation
    .AddSwaggerConfig()         // Swagger Documentation & Security
    .AddInfrastructureConfig(builder.Configuration, builder.Environment) // DB, Redis, RabbitMQ, HealthChecks
    .AddAuthConfig(builder.Configuration)           // JWT, ApiKey, Token Services
    .AddApplicationServices();  // Repositories, Use Cases, Core Services

var app = builder.Build();

// --- MIDDLEWARE PIPELINE ---

// --- INITIALIZATION TOOLS ---
app.UseForwardedHeaders();

// --- DATABASE MIGRATION & SEEDING ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        Log.Information(" >> Starting database migration... << ");
        
        if (!app.Environment.IsEnvironment("Testing"))
        {
            if (context.Database.GetPendingMigrations().Any())
            {
                await context.Database.MigrateAsync();
                Log.Information(" >> The database has been successfully updated. << ");
            }
        }
        
        Log.Information(" >> Starting database seeding... << ");
        await DbSeeder.SeedAsync(context);
        Log.Information(" >> Database seeding completed. << ");

    }
    catch (Exception ex)
    {
        Log.Error(ex, " >> An error occurred during database migration or seeding. << ");
    }
}


// Security & Optimization
app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Documentation
app.UseSwaggerConfig();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Monitoring & Diagnostics
app.UseSerilogRequestLogging();
app.MapHealthChecks("/health");

// Endpoints
app.MapControllers();

app.Run();

public partial class Program { }