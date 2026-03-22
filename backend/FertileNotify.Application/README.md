# FertileNotify.Application

The Application layer contains the business logic orchestration and use case implementations for the Fertile Notify system. It defines how the application should behave and coordinates between the Domain and Infrastructure layers.

## Overview

This project implements the use cases and application services that process notification events, manage templates, and coordinate the notification delivery workflow. It is built using **.NET 10** and follows the Command pattern to implement business workflows.

## Key Components

### Use Cases (Commands/Handlers)

The Application layer implements all major business operations, including:

#### Authentication
- **LoginHandler**: Authenticates subscribers and manages the login flow.
- **VerifyCodeHandler**: Processes and validates One-Time Password (OTP) codes.
- **RefreshTokenHandler**: Securely handles JWT token renewal.
- **UpdatePasswordHandler**: Manages secure password updates.
- **ForgotPasswordHandler**: Handles the password recovery process.

#### Subscriber Management
- **RegisterSubscriberHandler**: Creates and configures new subscriber accounts.
- **UpdateCompanyNameHandler**: Modifies subscriber organization details.
- **UpdateContactInfoHandler**: Updates email or phone contact information.
- **ManageChannelsHandler**: Configures active notification delivery channels.
- **SetChannelSettingHandler**: Manages fine-grained per-channel configurations.
- **UnsubscribeHandler**: Processes recipient opt-out requests.

#### API Key Management
- **CreateApiKeyHandler**: Generates and hashes new secure API keys.
- **RevokeApiKeyHandler**: Instantly revokes an active API key.

#### Notification Processing
- **SendNotificationHandler**: Orchestrates the rendering and delivery of single notifications.
- **NotificationComplaintHandler**: Processes complaints and feedback from recipients.

### Services

- **TemplateEngine**: Processes dynamic content using `{{placeholder}}` syntax. Supports complex payload data and formatting.
- **ApiKeyService**: Handles the secure generation and validation of API keys.
- **NotificationLogService**: Manages historical logs for all notification delivery attempts.
- **StatisticsService**: Provides real-time and historical usage analytics and quota tracking.

### Interfaces

The Application layer defines contracts that are implemented by the Infrastructure layer, following the Dependency Inversion Principle.

**Repositories:**
- `ISubscriberRepository`: Data access for subscriber accounts.
- `ISubscriptionRepository`: Plan management and usage tracking.
- `ITemplateRepository`: Notification template storage and retrieval.
- `IApiKeyRepository`: Secure storage for API key metadata and hashes.
- `IFatalRecipientRepository`: Management of blocked recipients.
- `INotificationLogRepository`: Access to delivery history logs.
- `IStatsRepository`: Aggregated usage and performance statistics.

**Services:**
- `INotificationSender`: Abstraction for multi-channel notification delivery (Email, SMS, Push, etc.).
- `ITokenService`: JWT token generation, parsing, and validation.
- `IOtpService`: Secure OTP generation and verification for 2FA login.
- `IEmailService`: Specialized service for complex email operations (e.g., MJML rendering).

## Notification Workflow

1. **Reception**: A command is received from the API layer (e.g., `SendNotificationCommand`).
2. **Validation**: FluentValidation ensures the input is correct.
3. **Domain Interaction**: The handler interacts with Domain entities (e.g., `Subscriber`, `Subscription`) to enforce business rules.
4. **Processing**: `TemplateEngine` renders the final content using event payload data.
5. **Infrastructure**: The handler calls interfaces like `INotificationSender` or repositories to persist changes or dispatch notifications.
6. **Result**: A structured result (Success/Failure) is returned to the API layer.

## Template Engine

The `TemplateEngine` supports sophisticated placeholder replacement:
- **Simple Syntax**: `{{name}}`
- **Nested Data**: `{{order.id}}`
- **Safe Handling**: Gracefully handles missing variables without crashing.

## Error Handling

The Application layer provides centralized error handling for:
- Missing subscribers or resources.
- Exceeded subscription quotas.
- Invalid or expired security tokens.
- Validation failures for input commands.

## Dependencies

- **FertileNotify.Domain**: Core business logic and rules.
- **FluentValidation**: Sophisticated input validation for all commands.
- **MassTransit**: Integrated for asynchronous message-based communication.
- **Microsoft.Extensions.Logging**: Structured logging across all use cases.

## Testing

Use case handlers are thoroughly tested using **Moq** to isolate dependencies. Test scenarios include:
- Validating correct orchestration of Domain objects.
- Ensuring repository methods are called with expected parameters.
- Verifying error responses for invalid business states (e.g., exceeded limits).
