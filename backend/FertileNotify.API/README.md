# FertileNotify.API

The API layer is the entry point for the Fertile Notify application, providing RESTful endpoints for notification delivery, subscriber management, and authentication. It supports dual authentication (JWT and API Keys) and follows Clean Architecture principles by delegating business logic to the Application layer.

## Overview

This project contains web API controllers, request/response models, validators, and middleware components. It is built using **.NET 10** and emphasizes performance, security, and developer productivity through Swagger/OpenAPI documentation.

## Key Components

### Controllers

- **AuthController**: Manages subscriber login flow, OTP verification, JWT token refresh, and password updates.
- **NotificationsController**: Handles the orchestration of single and bulk notification delivery requests.
- **SubscribersController**: Manages subscriber registration, profile retrieval, and active notification channels.
- **StatisticsController**: Provides endpoints for accessing real-time usage metrics and quota tracking.
- **LogController**: Allows subscribers to retrieve historical notification delivery logs.
- **RecipientsController**: Manages recipient blacklist (forbidden recipients) and opt-out requests.
- **TemplatesController**: Management of reusable notification templates.

### Models (DTOs)

The API project contains structured Request and Response objects (DTOs) organized by feature:
- **Authentication**: `LoginRequest`, `VerifyOtpRequest`, `RefreshTokenRequest`.
- **Subscriber**: `RegisterSubscriberRequest`, `UpdateChannelsRequest`, `GenerateApiKeyRequest`.
- **Notification**: `SendNotificationRequest`, `BulkNotificationRequest`.
- **Statistics**: `DailyStatsResponse`, `UsageSummaryResponse`.

### Validators

FluentValidation is used for robust request validation:
- **Input Integrity**: Ensures that email formats, password complexity, and title/message lengths meet system requirements.
- **Security Validation**: Validates OTP codes and API key generation metadata.
- **Business Rule Pre-checks**: Validates that required fields are present before passing commands to handlers.

### Middlewares

- **Global Error Handling**: Centralized exception handling that transforms application errors into standardized JSON responses.
- **Request Logging**: Serilog integration for structured logging of all incoming API requests and outgoing responses.
- **Rate Limiting**: Plan-based rate limiting to prevent API abuse.

### Authentication

A dual authentication system that supports:
- **JWT Bearer Tokens**: For interactive front-end applications, supporting OTP-based login and token refresh.
- **API Keys**: For secure server-to-server integration, with hashed storage and instant revocation.

## API Endpoints

### Authentication
- `POST /api/auth/login`: Email/password login flow (generates OTP).
- `POST /api/auth/verify-otp`: Secure 2FA verification (returns JWT).
- `POST /api/auth/refresh-token`: Renew expired access tokens.
- `PUT /api/auth/password`: Update security credentials.

### Subscribers
- `POST /api/subscribers/register`: New account creation with plan selection.
- `GET /api/subscribers/profile`: Authenticated profile retrieval.
- `PUT /api/subscribers/channels`: Management of active delivery channels.
- `POST /api/subscribers/api-keys`: Create secure API keys.
- `DELETE /api/subscribers/api-keys/{id}`: Instant key revocation.

### Notifications
- `POST /api/notifications/send`: High-performance notification dispatch.
- `POST /api/notifications/bulk`: Efficiently send messages to multiple recipients.

### Analytics & Logs
- `GET /api/statistics`: Usage summary and quota tracking.
- `GET /api/logs`: Access delivery history and status updates.

## Configuration

The API is configured using `appsettings.json` and environment variables. Key sections:
- `ConnectionStrings`: PostgreSQL connection info.
- `Redis:ConnectionString`: Distributed cache settings.
- `RabbitMQ`: Message broker credentials and host.
- `JwtSettings`: Security keys and token expiration (default: 1440 minutes).
- `RateLimiting`: Plan-specific request limits.

## Dependencies

- **ASP.NET Core 10.0**: High-performance web framework.
- **Swashbuckle.AspNetCore**: Automated Swagger/OpenAPI documentation.
- **FluentValidation.AspNetCore**: Expressive input validation.
- **Microsoft.AspNetCore.Authentication.JwtBearer**: Standardized JWT support.
- **Serilog.AspNetCore**: Structured logging with file and console sinks.
- **DotNetEnv**: Environment variable management.

## Running the API

### Development Mode
```bash
cd backend/FertileNotify.API
dotnet run
```
The API starts at `https://localhost:5001`. Access documentation at `/swagger`.

### Docker
```bash
docker build -t fertile-notify-api -f Dockerfile .
docker run -p 5080:8080 fertile-notify-api
```

## Performance & Scaling

- **Plan-Based Rate Limiting**: Ensures fair resource usage (Free: 50/min, Pro: 100/min, Enterprise: 1000/min).
- **Asynchronous Handlers**: All controller actions are async to prevent thread starvation.
- **Scalable Architecture**: The API can be horizontally scaled, with Redis and RabbitMQ coordinating state across instances.
