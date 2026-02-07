# Fertile Notify - Backend

A robust notification management system built with Clean Architecture principles using .NET 9, designed to handle multi-channel notifications (Email, SMS, In-App) with subscription-based access control and rate limiting.

## Overview

The Fertile Notify backend is a notification processing platform that allows subscribers to send notifications through multiple channels based on their subscription plan. The system is built using Clean Architecture with clear separation of concerns across four main layers.

## Architecture

The backend follows **Clean Architecture** (also known as Onion Architecture) with the following layers:

```
┌─────────────────────────────────────────────────┐
│           FertileNotify.API (Presentation)      │
│   Controllers, Authentication, Validation       │
└────────────────────┬────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────┐
│       FertileNotify.Application (Use Cases)     │
│   Handlers, Services, Interface Definitions     │
└────────────────────┬────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────┐
│         FertileNotify.Domain (Business)         │
│   Entities, Value Objects, Business Rules       │
└─────────────────────────────────────────────────┘
          ▲
          │ implements interfaces
┌─────────┴───────────────────────────────────────┐
│    FertileNotify.Infrastructure (Technical)     │
│   Database, Email, SMS, Background Jobs         │
└─────────────────────────────────────────────────┘
```

### Layer Responsibilities

- **API Layer**: HTTP endpoints, request validation, authentication/authorization
- **Application Layer**: Business workflows, use case orchestration, service interfaces
- **Domain Layer**: Core business logic, entities, value objects, business rules
- **Infrastructure Layer**: Database access, external services, notification delivery, background workers

## Key Features

### 🔐 Authentication & Authorization
- **Dual Authentication**: JWT Bearer tokens + API Key authentication
- **OTP Verification**: Two-factor login with one-time passwords
- **Token Refresh**: Secure token renewal mechanism
- **API Key Management**: Generate, revoke, and manage API keys

### 📊 Subscription Plans
Three tiers with different capabilities and limits:

| Feature | Free | Pro | Enterprise |
|---------|------|-----|------------|
| Monthly Notification Limit | 100 | 1,000 | 10,000 |
| Rate Limit (requests/min) | 50 | 100 | 1,000 |
| Available Channels | Email | Email, SMS | Email, SMS, Console |
| Allowed Event Types | Basic events | Extended events | All events |

### 📬 Multi-Channel Notifications
- **Email**: SMTP-based email delivery
- **SMS**: SMS gateway integration
- **Console**: In-app/console notifications for testing

### 🎨 Template Engine
- Dynamic content replacement with `{{variable}}` syntax
- Event-based template selection
- Reusable templates for different notification types

### ⚡ Asynchronous Processing
- **Background Worker**: Hosted service for async notification delivery
- **In-Memory Queue**: Fast message queuing for notification processing
- **Retry Mechanism**: Automatic retry with exponential backoff for failed deliveries

## Project Structure

```
backend/
├── FertileNotify.API/              # Web API controllers and endpoints
│   ├── Controllers/                # AuthController, NotificationsController, SubscribersController
│   ├── Models/                     # Request/Response DTOs
│   ├── Validators/                 # FluentValidation validators
│   ├── Middlewares/                # Exception handling
│   └── Authentication/             # JWT and API Key handlers
│
├── FertileNotify.Application/      # Business use cases and services
│   ├── UseCases/                   # ProcessEvent, RegisterSubscriber handlers
│   ├── Services/                   # TemplateEngine, OtpService
│   ├── Interfaces/                 # Repository and service contracts
│   └── DTOs/                       # Data transfer objects
│
├── FertileNotify.Domain/           # Core business logic
│   ├── Entities/                   # Subscriber, Notification, Subscription, etc.
│   ├── ValueObjects/               # EmailAddress, Password, PhoneNumber, etc.
│   ├── Enums/                      # SubscriptionPlan
│   ├── Rules/                      # Business rules and policies
│   ├── Events/                     # Domain events
│   └── Exceptions/                 # Domain-specific exceptions
│
├── FertileNotify.Infrastructure/   # External concerns implementation
│   ├── Persistence/                # EF Core DbContext, repositories
│   ├── Notifications/              # Email, SMS, Console senders
│   ├── BackgroundJobs/             # NotificationWorker, queue
│   ├── Authentication/             # JWT service, API key service
│   └── Migrations/                 # Database migrations
│
├── FertileNotify.Tests/            # Unit and integration tests
│   ├── Application/                # Use case handler tests
│   ├── Domain/                     # Entity and value object tests
│   └── Infrastructure/             # Repository tests (planned)
│
├── FertileNotify.sln               # Solution file
├── Dockerfile                      # Docker container definition
└── README.md                       # This file
```

## Getting Started

### Prerequisites
- .NET 9 SDK
- PostgreSQL 14+ database
- (Optional) Docker for containerized deployment

### Configuration

Create an `appsettings.Development.json` file in the `FertileNotify.API` directory:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=FertileNotifyDb;Username=postgres;Password=your_password"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-at-least-32-characters-long",
    "Issuer": "FertileNotify",
    "Audience": "FertileNotifyClients",
    "ExpiryInMinutes": 1440
  },
  "EmailSettings": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "Username": "your-email@example.com",
    "Password": "your-smtp-password",
    "FromAddress": "notifications@example.com",
    "FromName": "Fertile Notify"
  }
}
```

### Database Setup

1. Ensure PostgreSQL is running
2. Update connection string in `appsettings.json`
3. Apply migrations:

```bash
cd backend/FertileNotify.API
dotnet ef database update --project ../FertileNotify.Infrastructure
```

### Running the Application

#### Development Mode
```bash
cd backend/FertileNotify.API
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

#### Watch Mode (Auto-reload on changes)
```bash
dotnet watch run
```

#### Production Mode
```bash
dotnet run --configuration Release
```

### Using Docker

Build and run the containerized application:

```bash
cd backend
docker build -t fertile-notify-api .
docker run -p 5080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Database=FertileNotifyDb;Username=postgres;Password=password" \
  fertile-notify-api
```

Or use Docker Compose from the root directory:

```bash
cd ..
docker-compose up
```

## API Endpoints

### Authentication
- `POST /api/auth/login` - Login with email/password, get OTP
- `POST /api/auth/verify-otp` - Verify OTP and receive JWT token
- `POST /api/auth/refresh-token` - Refresh expired JWT token
- `PUT /api/auth/password` - Update password

### Subscribers
- `POST /api/subscribers/register` - Register new subscriber
- `GET /api/subscribers/profile` - Get current subscriber profile
- `PUT /api/subscribers/channels` - Update notification channel preferences
- `POST /api/subscribers/api-keys` - Generate new API key
- `GET /api/subscribers/api-keys` - List all API keys
- `DELETE /api/subscribers/api-keys/{id}` - Revoke API key

### Notifications
- `POST /api/notifications/send` - Send single notification
- `POST /api/notifications/bulk` - Send bulk notifications

## Running Tests

### All Tests
```bash
cd backend
dotnet test
```

### Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~ProcessEventHandlerTests"
```

### With Code Coverage
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Watch Mode
```bash
dotnet watch test
```

## Technology Stack

- **.NET 9**: Latest .NET framework
- **ASP.NET Core**: Web API framework
- **Entity Framework Core 9**: ORM for database access
- **PostgreSQL**: Relational database
- **FluentValidation**: Request validation
- **JWT Bearer**: Token-based authentication
- **BCrypt**: Password hashing
- **Serilog**: Structured logging
- **xUnit/NUnit**: Testing frameworks
- **Moq**: Mocking framework
- **Swagger/OpenAPI**: API documentation

## Development Guidelines

### Code Organization
- Follow Clean Architecture principles
- Keep layers independent with clear boundaries
- Use dependency injection for loose coupling
- Implement repository pattern for data access
- Apply SOLID principles throughout

### Testing Strategy
- **Domain Layer**: Unit tests for business logic (90%+ coverage)
- **Application Layer**: Use case tests with mocked dependencies (85%+ coverage)
- **Infrastructure Layer**: Integration tests with test database (70%+ coverage)
- **API Layer**: Controller tests for endpoints

### Database Migrations
```bash
# Create new migration
dotnet ef migrations add MigrationName --project FertileNotify.Infrastructure --startup-project FertileNotify.API

# Apply migrations
dotnet ef database update --project FertileNotify.Infrastructure --startup-project FertileNotify.API

# Rollback migration
dotnet ef database update PreviousMigrationName --project FertileNotify.Infrastructure --startup-project FertileNotify.API
```

## Business Rules

### Subscription Limits
- **Free Plan**: 100 notifications/month, Email only, 50 requests/min
- **Pro Plan**: 1,000 notifications/month, Email + SMS, 100 requests/min
- **Enterprise Plan**: 10,000 notifications/month, All channels, 1,000 requests/min

### Notification Channels
- Subscribers can enable up to 3 notification channels
- Channel availability depends on subscription plan
- Email is available for all plans

### Rate Limiting
- Enforced at the API level based on subscription plan
- Uses sliding window algorithm
- Returns 429 (Too Many Requests) when limit exceeded

### Retry Policy
- Failed notifications retry up to 3 times
- Exponential backoff: 30s, 2min, 5min
- After max retries, notification marked as permanently failed

## Monitoring and Logging

The application uses Serilog for structured logging with the following log levels:

- **Debug**: Development diagnostics
- **Information**: Application flow, successful operations
- **Warning**: Retry attempts, rate limit warnings
- **Error**: Failed operations, exceptions
- **Critical**: System failures

Logs are written to:
- Console (development)
- File (production) - `/logs` directory
- Application Insights (if configured)

## Contributing

1. Follow the existing code structure and patterns
2. Write unit tests for new features
3. Update relevant README files
4. Ensure all tests pass before committing
5. Follow C# coding conventions

## License

[MIT License](../LICENSE)

## Support

For issues, questions, or contributions, please refer to the main repository documentation.
