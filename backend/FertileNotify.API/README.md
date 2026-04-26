# FertileNotify.API

The API layer is the entry point for the Fertile Notify application, providing RESTful endpoints for notification delivery, subscriber management, authentication, and payment processing. It supports dual authentication (JWT and API Keys) and follows Clean Architecture principles by delegating business logic to the Application layer.

## Overview

This project contains web API controllers, request/response models, validators, middleware components, and service registration extensions. It is built using **.NET 10** and emphasizes performance, security, and developer productivity through Swagger/OpenAPI documentation.

## Key Components

### Controllers

- **AuthController**: Manages subscriber login flow, OTP verification, JWT token refresh, and password recovery.
- **NotificationsController**: Handles single notification dispatch and workflow notification management (create, update, trigger, list, delete, activate/deactivate).
- **SubscribersController**: Manages subscriber registration, profile updates, API key management (including scopes and status), per-channel settings, extra-credit top-ups, and data export.
- **StatisticsController**: Provides endpoints for accessing real-time usage metrics and quota tracking.
- **LogController**: Allows subscribers to retrieve historical notification delivery logs.
- **RecipientsController**: Manages recipient blacklist (forbidden recipients), opt-out requests, and complaint reporting.
- **TemplatesController**: Management of reusable notification templates (list, query, create/update custom).
- **PaymentsController**: Handles Stripe payment intent creation for extra credits, payment history retrieval, and Stripe webhook processing.
- **SystemNotificationsController**: Manages in-app system notifications for subscribers (list, filter by read/unread status, mark as read).

### Models (DTOs)

Request and Response objects are organized by feature under `Models/Requests/` and `Models/Responses/`:
- **Authentication**: `UserLoginRequest`, `OtpRequest`, `RefreshTokenRequest`, `ForgotPasswordRequest`, `UpdatePasswordRequest`, `UserResetPasswordRequest`, `CreateApiKeyRequest`, `UpdateApiKeyScopesRequest`, `UpdateApiKeyStatusRequest`.
- **Subscriber**: `RegisterSubscriberRequest`, `UpdateSubscriberProfileRequest`, `ManageChannelRequest`, `ChannelSettingRequest`.
- **Notification**: `SendNotificationRequest`, `AddWorkflowNotificationRequest`, `UpdateWorkflowNotificationRequest`.
- **Templates**: `CreateTemplateRequest`, `GetTemplatesRequest`.
- **Recipients**: `UnsubscribeRequest`, `BanRecipientRequest`, `UpdateBanRequest`, `ComplaintRequest`.
- **Payment**: `CreateExtraCreditPaymentIntentRequest`.
- **Common**: `ApiResponse<T>` — standardized response wrapper used across all endpoints.

### Validators

FluentValidation is used for robust request validation (organized by feature under `Validators/`):
- **Input Integrity**: Ensures that email formats, password complexity, and title/message lengths meet system requirements.
- **Security Validation**: Validates OTP codes and API key generation metadata.
- **Business Rule Pre-checks**: Validates that required fields are present before passing commands to handlers.

### Middlewares

- **ExceptionHandlingMiddleware**: Centralized exception handling that transforms application errors into standardized JSON responses with error codes.

### Authentication

A dual authentication system that supports:
- **JWT Bearer Tokens**: For interactive front-end applications, supporting OTP-based login and token refresh (`ApiKeyAuthenticationHandler`).
- **API Keys**: For secure server-to-server integration, with hashed storage, scope management, and instant revocation.

### Authorization

- **ApiKeyScopePolicies**: Defines and enforces scope-based access control for API key authenticated requests.

### Extensions

Service registration extensions that keep `Program.cs` clean:
- **ApplicationServiceExtension**: Registers MediatR, FluentValidation, and application services.
- **AuthExtension**: Configures JWT and API key authentication schemes.
- **InfrastructureExtension**: Registers EF Core, Redis, RabbitMQ, and infrastructure services.
- **SwaggerExtension**: Configures Swagger/OpenAPI with authentication support.
- **WebExtension**: Configures CORS, rate limiting, and HTTP pipeline.
- **EnvExtension**: Loads `.env` file environment variables.

## API Endpoints

### Authentication
- `POST /api/auth/login`: Email/password login flow (generates OTP).
- `POST /api/auth/verify-code`: Secure 2FA OTP verification (returns JWT + refresh token).
- `POST /api/auth/refresh-token`: Renew expired access tokens.
- `POST /api/auth/forgot-password`: Send a password reset OTP to the subscriber's email.

### Subscribers
- `POST /api/subscribers/register`: New account creation.
- `GET /api/subscribers/me`: Authenticated profile and subscription info retrieval.
- `PUT /api/subscribers/profile`: Update subscriber profile (company name, contact info).
- `POST /api/subscribers/channels`: Manage active notification delivery channels.
- `PUT /api/subscribers/password`: Update security credentials.
- `DELETE /api/subscribers/delete-account`: Permanently delete the subscriber account.
- `POST /api/subscribers/api-keys`: Create a secure API key for server-to-server use.
- `GET /api/subscribers/api-keys`: List all active API keys.
- `PATCH /api/subscribers/api-keys/{apiKeyId}/scopes`: Update the allowed scopes of an API key.
- `PATCH /api/subscribers/api-keys/{apiKeyId}/status`: Enable or disable an API key.
- `DELETE /api/subscribers/api-keys/{apiKeyId}`: Instant key revocation.
- `POST /api/subscribers/settings/channel-setting`: Set per-channel configuration (e.g., webhook URL, SMTP creds).
- `GET /api/subscribers/settings/channel-setting`: Retrieve per-channel configuration by channel name.
- `PATCH /api/subscribers/add-extra-credits`: Manually add extra notification credits (admin use).
- `GET /api/subscribers/export-data`: Export all subscriber data in a portable format.

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

### Payments
- `POST /api/payments/extra-credits/intent`: Create a Stripe payment intent to purchase additional notification credits.
- `GET /api/payments/history`: Retrieve the subscriber's payment history.
- `POST /api/payments/webhook`: Stripe webhook endpoint (processes successful payment events, no auth required).

### System Notifications
- `GET /api/system-notifications`: List in-app system notifications. Supports `?status=all|read|unread` filter.
- `PATCH /api/system-notifications/{notificationId}/read`: Mark a specific system notification as read.

## Configuration

The API is configured using `appsettings.json` and environment variables. Key sections:
- `ConnectionStrings`: PostgreSQL connection info.
- `Redis:ConnectionString`: Distributed cache settings.
- `RabbitMQ`: Message broker credentials and host.
- `JwtSettings`: Security keys and token expiration (default: 1440 minutes).
- `RateLimiting`: Plan-specific request limits.
- `Stripe`: Secret key and webhook signing secret for payment processing.

## Dependencies

- **ASP.NET Core 10.0**: High-performance web framework.
- **Swashbuckle.AspNetCore**: Automated Swagger/OpenAPI documentation.
- **FluentValidation.AspNetCore**: Expressive input validation.
- **Microsoft.AspNetCore.Authentication.JwtBearer**: Standardized JWT support.
- **Serilog.AspNetCore**: Structured logging with file and console sinks.
- **DotNetEnv**: Environment variable management.
- **Stripe.net**: Stripe payment integration.

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
