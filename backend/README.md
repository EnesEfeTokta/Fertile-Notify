# Fertile Notify - Backend

A robust notification management system built with Clean Architecture principles using .NET 10, designed to handle multi-channel notifications (Email, SMS, Push, Slack, Discord, MS Teams, etc.) with subscription-based access control and rate limiting.

## Overview

The Fertile Notify backend is a high-performance notification processing platform that allows subscribers to send notifications through multiple channels based on their subscription plan. The system is built using Clean Architecture with clear separation of concerns across four main layers, ensuring maintainability and scalability.

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
│   Database, External Services, Background Jobs  │
└─────────────────────────────────────────────────┘
```

### Layer Responsibilities

- **API Layer**: HTTP endpoints, request validation, authentication/authorization, and rate limiting.
- **Application Layer**: Business workflows, use case orchestration, and service interfaces.
- **Domain Layer**: Core business logic, entities, value objects, and fundamental business rules.
- **Infrastructure Layer**: Data persistence, external notification services, background consumers, and caching.

## Key Features

### 🔐 Authentication & Authorization
- **Dual Authentication**: JWT Bearer tokens for users and API Key authentication for server-to-server integration.
- **OTP Verification**: Secure two-factor login flow with one-time passwords.
- **Role-Based Access**: Granular control over features based on subscription tiers.

### 📊 Subscription Plans
Three tiers with different capabilities and limits:

| Feature | Free | Pro | Enterprise |
|---------|------|-----|------------|
| Monthly Notification Limit | 100 | 1,000 | 10,000 |
| Rate Limit (requests/min) | 50 | 100 | 1,000 |
| Available Channels | Email | Email, SMS, Push | All Channels |
| Allowed Event Types | Basic events | Extended events | All events |

### 📬 Multi-Channel Notifications
Support for a wide range of delivery channels:
- **Email**: SMTP and advanced email services.
- **Mobile/Web**: Firebase Push Notifications, Web Push.
- **Messaging**: WhatsApp, SMS, Telegram.
- **Collaboration**: Slack, Discord, MS Teams.
- **Integrations**: Webhooks, Console (for testing).

### 🎨 Template Engine
- Dynamic content replacement with `{{variable}}` syntax.
- Support for MJML for responsive email templates.
- Event-based template selection and channel-specific formatting.

### ⚡ Advanced Infrastructure
- **Message Broker**: MassTransit with RabbitMQ for reliable asynchronous processing.
- **Caching**: Distributed caching using Redis for performance optimization.
- **Background Processing**: Dedicated consumers for handling high-volume notification traffic.

## Project Structure

```
backend/
├── FertileNotify.API/              # Web API controllers and endpoints
│   ├── Controllers/                # RESTful endpoints (Auth, Notifications, Subscribers, etc.)
│   ├── Models/                     # Request/Response DTOs
│   ├── Validators/                 # FluentValidation logic
│   └── Authentication/             # Auth handlers and JWT/API Key logic
│
├── FertileNotify.Application/      # Business use cases and services
│   ├── UseCases/                   # Core business workflows (Commands/Handlers)
│   ├── Services/                   # Domain services (TemplateEngine, Stats, etc.)
│   └── Interfaces/                 # Repository and external service contracts
│
├── FertileNotify.Domain/           # Core business logic
│   ├── Entities/                   # Domain models (Subscriber, Subscription, etc.)
│   ├── ValueObjects/               # Immutable business components
│   └── Rules/                      # Domain-specific business rules
│
├── FertileNotify.Infrastructure/   # Technical implementations
│   ├── Persistence/                # EF Core, PostgreSQL, Repositories
│   ├── Notifications/              # Concrete sender implementations (Email, SMS, etc.)
│   └── BackgroundJobs/             # MassTransit consumers and workers
│
├── FertileNotify.Tests/            # Unit and integration tests
│
├── FertileNotify.sln               # Solution file
└── README.md                       # This file
```

## Getting Started

### Prerequisites
- .NET 10 SDK
- PostgreSQL 15+
- RabbitMQ
- Redis
- (Optional) Docker

### Configuration

The application uses `appsettings.json` and environment variables for configuration. Create an `appsettings.Development.json` file in the `FertileNotify.API` directory:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=FertileNotifyDb;Username=postgres;Password=your_password"
  },
  "Redis": {
    "ConnectionString": "localhost:6379,abortConnect=false"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "guest",
    "Password": "guest"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-at-least-32-characters-long",
    "Issuer": "FertileNotifyAPI",
    "Audience": "FertileNotifyClients",
    "ExpiryInMinutes": 15
  },
  "SystemSmtp": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "your-email@example.com",
    "Password": "your-smtp-password",
    "From": "notifications@example.com"
  }
}
```

### Database Setup

1. Ensure PostgreSQL is running.
2. Update connection string in `appsettings.json`.
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
The API will be available at `https://localhost:5001/swagger`.

#### Watch Mode (Auto-reload on changes)
```bash
dotnet watch run
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

## API Endpoints Summary

### Authentication
- `POST /api/auth/login`: Login with email/password, generates OTP.
- `POST /api/auth/verify-otp`: Verify OTP code and receive JWT token.
- `POST /api/auth/refresh-token`: Refresh expired JWT token.
- `PUT /api/auth/password`: Update password.

### Subscribers
- `POST /api/subscribers/register`: Register new subscriber.
- `GET /api/subscribers/profile`: Get current subscriber profile.
- `PUT /api/subscribers/channels`: Update notification channel preferences.
- `POST /api/subscribers/api-keys`: Generate new API key.
- `DELETE /api/subscribers/api-keys/{id}`: Revoke API key.

### Notifications
- `POST /api/notifications/send`: Send single notification.
- `POST /api/notifications/bulk`: Send bulk notifications.

## Running Tests

```bash
cd backend
dotnet test
```

## Technology Stack

- **Framework**: .NET 10 / ASP.NET Core
- **Database**: PostgreSQL / Entity Framework Core 10
- **Messaging**: MassTransit / RabbitMQ
- **Caching**: Redis
- **Validation**: FluentValidation
- **Logging**: Serilog
- **Auth**: JWT / API Keys / BCrypt
- **Email**: MJML.NET / MailKit
- **Testing**: xUnit / Moq / FluentAssertions

## License

[MIT License](../LICENSE)
