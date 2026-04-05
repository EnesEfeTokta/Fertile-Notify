# FertileNotify.API

The API layer is the entry point for the Fertile Notify application, providing RESTful endpoints for notification delivery, subscriber management, and authentication. It supports dual authentication (JWT and API Keys) and follows Clean Architecture principles by delegating business logic to the Application layer.

## Overview

This project contains web API controllers, request/response models, validators, and middleware components. It is built using **.NET 10** and emphasizes performance, security, and developer productivity through Swagger/OpenAPI documentation.

## Key Components

### Controllers

- **AuthController**: Manages subscriber login flow, OTP verification, JWT token refresh, and password recovery.
- **NotificationsController**: Handles single notification dispatch and workflow notification management (create, update, trigger, list, delete, activate/deactivate).
- **SubscribersController**: Manages subscriber registration, profile retrieval, contact/company updates, API key management, and per-channel settings.
- **StatisticsController**: Provides endpoints for accessing real-time usage metrics and quota tracking.
- **LogController**: Allows subscribers to retrieve historical notification delivery logs.
- **RecipientsController**: Manages recipient blacklist (forbidden recipients), opt-out requests, and complaint reporting.
- **TemplatesController**: Management of reusable notification templates (list, query, create/update custom).

### Models (DTOs)

The API project contains structured Request and Response objects (DTOs) organized by feature:
- **Authentication**: `UserLoginRequest`, `OtpRequest`, `RefreshTokenRequest`, `ForgotPasswordRequest`, `UpdatePasswordRequest`, `UserResetPasswordRequest`.
- **Subscriber**: `RegisterSubscriberRequest`, `UpdateContactRequest`, `UpdateCompanyNameRequest`, `ManageChannelRequest`, `ChannelSettingRequest`, `CreateApiKeyRequest`.
- **Notification**: `SendNotificationRequest`, `AddWorkflowNotificationRequest`, `UpdateWorkflowNotificationRequest`.
- **Templates**: `CreateTemplateRequest`, `GetTemplatesRequest`.
- **Recipients**: `UnsubscribeRequest`, `BanRecipientRequest`, `UpdateBanRequest`, `ComplaintRequest`.

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
- `POST /api/auth/verify-code`: Secure 2FA OTP verification (returns JWT).
- `POST /api/auth/refresh-token`: Renew expired access tokens.
- `POST /api/auth/forgot-password`: Send a password reset OTP to the subscriber's email.

### Subscribers
- `POST /api/subscribers/register`: New account creation.
- `GET /api/subscribers/me`: Authenticated profile and subscription info retrieval.
- `PUT /api/subscribers/contact`: Update contact email or phone number.
- `PUT /api/subscribers/company-name`: Update organization name.
- `POST /api/subscribers/channels`: Manage active notification delivery channels.
- `PUT /api/subscribers/password`: Update security credentials.
- `DELETE /api/subscribers/delete-account`: Permanently delete the subscriber account.
- `POST /api/subscribers/create-api-key`: Create a secure API key for server-to-server use.
- `GET /api/subscribers/api-keys`: List all active API keys.
- `DELETE /api/subscribers/api-keys/{apiKeyId}`: Instant key revocation.
- `POST /api/subscribers/settings/channel-setting`: Set per-channel configuration (e.g., webhook URL, SMTP creds).
- `GET /api/subscribers/settings/channel-setting`: Retrieve per-channel configuration by channel name.

### Notifications
- `POST /api/notifications/send`: Dispatch a notification to one or more recipients across multiple channels.
- `POST /api/notifications/workflow/send/{eventTrigger}`: Trigger workflow notifications by event name.
- `POST /api/notifications/workflow/add`: Create a new scheduled/triggered workflow notification.
- `PUT /api/notifications/workflow/update`: Update an existing workflow notification.
- `GET /api/notifications/workflow/list`: List all workflow notifications for the subscriber.
- `GET /api/notifications/workflow/get/{id}`: Retrieve a specific workflow notification by ID.
- `DELETE /api/notifications/workflow/delete/{id}`: Delete a workflow notification.
- `POST /api/notifications/workflow/activate/{id}`: Activate a workflow notification.
- `POST /api/notifications/workflow/deactivate/{id}`: Deactivate a workflow notification.

### Templates
- `GET /api/templates`: List available notification templates.
- `POST /api/templates/query`: Query templates with filters (channel, event type, etc.).
- `POST /api/templates/create-or-update-custom`: Create or update a custom subscriber-owned template.

### Recipients
- `POST /api/recipients/unsubscribe`: Process a recipient opt-out request.
- `POST /api/recipients/complaint`: Submit a notification complaint from a recipient.
- `GET /api/recipients/complaints`: Retrieve a list of received complaints.
- `GET /api/recipients/blacklist`: List all blacklisted recipients.
- `POST /api/recipients/blacklist`: Manually add a recipient to the blacklist.
- `DELETE /api/recipients/blacklist/{id}`: Remove a recipient from the blacklist.

### Analytics & Logs
- `GET /api/statistics`: Usage summary and quota tracking for the authenticated subscriber.
- `GET /api/logs/{limit}`: Access recent notification delivery history and status updates.

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
