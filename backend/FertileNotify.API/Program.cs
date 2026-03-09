using FertileNotify.API.Authentication;
using FertileNotify.API.Middlewares;
using FertileNotify.Application.Interfaces;
using FertileNotify.Application.Services;
using FertileNotify.Application.UseCases.ProcessEvent;
using FertileNotify.Application.UseCases.RegisterSubscriber;
using FertileNotify.Infrastructure.Authentication;
using FertileNotify.Infrastructure.BackgroundJobs;
using FertileNotify.Infrastructure.Notifications;
using FertileNotify.Infrastructure.Persistence;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mjml.Net;
using Serilog;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

DotNetEnv.Env.Load();

var envValues = new Dictionary<string, string?>();

// --- JWT Mapping ---
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("JWT_SECRET")))
    envValues["JwtSettings:SecretKey"] = Environment.GetEnvironmentVariable("JWT_SECRET");

// --- Database Mapping ---
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPass = Environment.GetEnvironmentVariable("DB_PASS");
if (!string.IsNullOrEmpty(dbUser) && !string.IsNullOrEmpty(dbPass))
{
    var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "localhost";
    var dbPort = Environment.GetEnvironmentVariable("DB_PORT") ?? "5432";
    var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "FertileNotifyDb";
    envValues["ConnectionStrings:DefaultConnection"] = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPass};";
}

// --- Redis Mapping ---
var redisHost = Environment.GetEnvironmentVariable("REDIS_HOST");
if (!string.IsNullOrEmpty(redisHost))
{
    var redisPort = Environment.GetEnvironmentVariable("REDIS_PORT") ?? "6379";
    envValues["Redis:ConnectionString"] = $"{redisHost}:{redisPort},abortConnect=false";
}

// --- RabbitMQ Mapping ---
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RABBITMQ_HOST")))
    envValues["RabbitMQ:Host"] = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RABBITMQ_USER")))
    envValues["RabbitMQ:Username"] = Environment.GetEnvironmentVariable("RABBITMQ_USER");
if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("RABBITMQ_PASS")))
    envValues["RabbitMQ:Password"] = Environment.GetEnvironmentVariable("RABBITMQ_PASS");

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddInMemoryCollection(envValues);

// --- LOGGING ---
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
    .CreateLogger();

builder.Host.UseSerilog();

// --- CORS ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("https://fertile-notify.enesefetokta.shop");

        if (builder.Environment.IsDevelopment())
        {
            policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
                  .AllowCredentials();
        }

        policy.AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- RATE LIMITER ---
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var subscriberId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var partitionKey = subscriberId ?? context.Connection.RemoteIpAddress?.ToString() ?? "anonymous";

        var plan = context.User.FindFirst("Plan")?.Value ?? "Free";

        int permitLimit = plan switch
        {
            "Pro" => 100,
            "Enterprise" => 1000,
            _ => 20
        };

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: partitionKey,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = permitLimit,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many requests. Please upgrade your plan.", token);
    };
});

// --- SWAGGER ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Fertile Notify API", Version = "v1" });
    c.CustomSchemaIds(type => type.FullName);
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key authentication using FN-Api-Key header. Example: \"FN-Api-Key: your-api-key\"",
        Name = "FN-Api-Key",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" },
                Scheme = "ApiKeyScheme",
                Name = "FN-Api-Key",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

// --- CONTROLLERS ---
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// --- DATABASE & EF ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!);

// --- REPOSITORIES ---
builder.Services.AddScoped<ISubscriberRepository, EfSubscriberRepository>();
builder.Services.AddScoped<ISubscriptionRepository, EfSubscriptionRepository>();
builder.Services.AddScoped<ITemplateRepository, EfTemplateRepository>();
builder.Services.AddScoped<IApiKeyRepository, EfApiKeyRepository>();
builder.Services.AddScoped<INotificationLogRepository, EfNotificationLogRepository>();
builder.Services.AddScoped<ISubscriberChannelRepository, EfSubscriberChannelRepository>();
builder.Services.AddScoped<IStatsRepository, EfStatsRepository>();

// --- NOTIFICATION LOG ---
builder.Services.AddScoped<IStatisticsService, StatisticsService>();

// --- BACKGROUND JOBS ---
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<NotificationConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h => {
            h.Username(builder.Configuration["RabbitMQ:Username"] ?? "guest");
            h.Password(builder.Configuration["RabbitMQ:Password"] ?? "guest");
        });

        cfg.ReceiveEndpoint("notification-queue", e => {
            e.ConfigureConsumer<NotificationConsumer>(context);
        });
    });
});

// --- VALIDATION ---
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// --- AUTH ---
builder.Services.AddScoped<ITokenService, JwtTokenService>();
builder.Services.AddScoped<ApiKeyService>();
builder.Services.AddScoped<IOtpService, OtpService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"] ?? "localhost:6379";
    options.InstanceName = "FertileNotify_";
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = "JWT_OR_APIKEY";
    options.DefaultChallengeScheme = "JWT_OR_APIKEY";
})
.AddJwtBearer("Bearer", options =>
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
})
.AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey", null)
.AddPolicyScheme("JWT_OR_APIKEY", "JWT_OR_APIKEY", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        if (context.Request.Headers.ContainsKey("FN-Api-Key"))
            return "ApiKey";
        return "Bearer";
    };
});

// --- SERVICES ---
builder.Services.AddHttpClient();

builder.Services.AddScoped<INotificationSender, ConsoleNotificationSender>();
builder.Services.AddScoped<INotificationSender, EmailNotificationSender>();
builder.Services.AddScoped<INotificationSender, SMSNotificationSender>();
builder.Services.AddScoped<INotificationSender, SlackNotificationSender>();
builder.Services.AddScoped<INotificationSender, FirebasePushNotificationSender>();
builder.Services.AddScoped<INotificationSender, WebPushNotificationSender>();

builder.Services.AddScoped<ProcessEventHandler>();
builder.Services.AddScoped<RegisterSubscriberHandler>();
builder.Services.AddScoped<TemplateEngine>(); 
builder.Services.AddSingleton<IMjmlRenderer, MjmlRenderer>();

builder.Services.AddScoped<INotificationSender, TelegramNotificationSender>();
builder.Services.AddScoped<INotificationSender, DiscordNotificationSender>();
builder.Services.AddScoped<INotificationSender, WhatsAppNotificationSender>();
builder.Services.AddScoped<INotificationSender, MSTeamsNotificationSender>();
builder.Services.AddScoped<INotificationSender, WebhookNotificationSender>();

// --- PROXY HEADERS ---
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
});

var app = builder.Build();

app.UseForwardedHeaders();

// --- SWAGGER ---
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Fertile Notify API v1");
});

// --- MIDDLEWARE PIPELINE ---

// Seeder
await DbSeeder.SeedAsync(app);

app.UseCors("AllowFrontend");
app.UseRateLimiter();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.UseSerilogRequestLogging();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();