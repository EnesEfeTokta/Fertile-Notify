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
    .AddInfrastructureConfig(builder.Configuration) // DB, Redis, RabbitMQ, HealthChecks
    .AddAuthConfig(builder.Configuration)           // JWT, ApiKey, Token Services
    .AddApplicationServices();  // Repositories, Use Cases, Core Services

var app = builder.Build();

// --- MIDDLEWARE PIPELINE ---

// Initial Tools
app.UseForwardedHeaders();
await DbSeeder.SeedAsync(app); // Seeder

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

// --- AUTOMATIC MIGRATION ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            await context.Database.MigrateAsync();
            Console.WriteLine(" >> The database has been successfully updated. << ");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, " >> An error occurred during database migration. << ");
    }
}

app.Run();