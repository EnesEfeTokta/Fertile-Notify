# Fertile Notify - Backend

A robust notification management system built with Clean Architecture principles using .NET 10, designed to handle multi-channel notifications (Email, SMS, Push, Slack, Discord, MS Teams, WhatsApp, Telegram, Webhooks, and more) with subscription-based access control, rate limiting, and Stripe-powered extra-credit top-ups.

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
│   ├── Authentication/             # JWT and API Key authentication handlers
│   ├── Authorization/              # API key scope policies
│   ├── Controllers/                # RESTful endpoints (Auth, Notifications, Subscribers,
│   │                               #   Templates, Recipients, Statistics, Log,
│   │                               #   Payments, SystemNotifications)
│   ├── Extensions/                 # Service registration extensions
│   ├── Middlewares/                # Exception handling, request logging
│   ├── Models/                     # Request/Response DTOs (by feature)
│   └── Validators/                 # FluentValidation logic (by feature)
│
├── FertileNotify.Application/      # Business use cases and services
│   ├── Contracts/                  # MassTransit message contracts
│   ├── DTOs/                       # Data Transfer Objects (by feature)
│   ├── Interfaces/                 # Repository and service contracts (by feature)
│   ├── Services/                   # Domain services (TemplateEngine, Statistics, etc.)
│   └── UseCases/                   # Core business workflows (Commands/Handlers by feature)
│
├── FertileNotify.Domain/           # Core business logic
│   ├── Entities/                   # Domain models (Subscriber, Subscription, PaymentLog, etc.)
│   ├── ValueObjects/               # Immutable business components (EmailAddress, CustomUrl, etc.)
│   └── Rules/                      # Domain-specific business rules
│
├── FertileNotify.Infrastructure/   # Technical implementations
│   ├── Authentication/             # JWT and OTP services
│   ├── BackgroundJobs/             # MassTransit consumers and hosted workers
│   ├── Notifications/              # Concrete sender implementations (Email, SMS, Push, etc.)
│   ├── Payment/                    # Stripe payment service
│   └── Persistence/                # EF Core, PostgreSQL, Repositories
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
- Stripe account (optional, for payment features)
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
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
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
- `POST /api/auth/verify-code`: Verify OTP code and receive JWT token.
- `POST /api/auth/refresh-token`: Refresh expired JWT token.
- `POST /api/auth/forgot-password`: Send password reset OTP.

### Subscribers
- `POST /api/subscribers/register`: Register new subscriber.
- `GET /api/subscribers/me`: Get current subscriber profile and subscription info.
- `PUT /api/subscribers/profile`: Update subscriber profile (company name, contact info).
- `POST /api/subscribers/channels`: Update active notification channels.
- `PUT /api/subscribers/password`: Update password.
- `DELETE /api/subscribers/delete-account`: Delete subscriber account.
- `POST /api/subscribers/api-keys`: Generate new API key.
- `GET /api/subscribers/api-keys`: List all API keys.
- `PATCH /api/subscribers/api-keys/{apiKeyId}/scopes`: Update API key scopes.
- `PATCH /api/subscribers/api-keys/{apiKeyId}/status`: Enable or disable an API key.
- `DELETE /api/subscribers/api-keys/{apiKeyId}`: Revoke API key.
- `POST /api/subscribers/settings/channel-setting`: Set per-channel configuration.
- `GET /api/subscribers/settings/channel-setting`: Get per-channel configuration.
- `PATCH /api/subscribers/add-extra-credits`: Manually add extra notification credits (admin).
- `GET /api/subscribers/export-data`: Export all subscriber data.

### Notifications
- `POST /api/notifications/send`: Send a notification to one or more recipients.
- `POST /api/notifications/workflow/send/{eventTrigger}`: Trigger a workflow notification by event.
- `POST /api/notifications/workflow/add`: Create a new workflow notification.
- `PUT /api/notifications/workflow/update`: Update an existing workflow notification.
- `GET /api/notifications/workflow/list`: List all workflow notifications.
- `GET /api/notifications/workflow/get/{id}`: Get a specific workflow notification.
- `DELETE /api/notifications/workflow/delete/{id}`: Delete a workflow notification.
- `POST /api/notifications/workflow/activate/{id}`: Activate a workflow notification.
- `POST /api/notifications/workflow/deactivate/{id}`: Deactivate a workflow notification.

### Templates
- `GET /api/templates`: List available templates.
- `POST /api/templates/query`: Query templates with filters.
- `POST /api/templates/create-or-update-custom`: Create or update a custom template.

### Recipients
- `POST /api/recipients/unsubscribe`: Unsubscribe a recipient.
- `POST /api/recipients/complaint`: Submit a notification complaint.
- `GET /api/recipients/complaints`: List complaints.
- `GET /api/recipients/blacklist`: List blacklisted recipients.
- `POST /api/recipients/blacklist`: Add a recipient to the blacklist.
- `DELETE /api/recipients/blacklist/{id}`: Remove a recipient from the blacklist.

### Statistics & Logs
- `GET /api/statistics`: Get usage statistics and quota tracking.
- `GET /api/logs/{limit}`: Get recent notification delivery logs.

### Payments
- `POST /api/payments/extra-credits/intent`: Create a Stripe payment intent for extra credits.
- `GET /api/payments/history`: Get payment history.
- `POST /api/payments/webhook`: Stripe webhook handler (processes successful payments).

### System Notifications
- `GET /api/system-notifications`: List in-app system notifications (filterable by status: all/read/unread).
- `PATCH /api/system-notifications/{notificationId}/read`: Mark a system notification as read.

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
- **Email**: MJML.Net / MailKit
- **Payments**: Stripe
- **Testing**: xUnit / Moq / FluentAssertions

## License

[GPL-3.0 License](../LICENSE)
