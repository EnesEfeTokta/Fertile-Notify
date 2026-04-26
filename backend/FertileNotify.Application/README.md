# FertileNotify.Application

The Application layer contains the business logic orchestration and use case implementations for the Fertile Notify system. It defines how the application should behave and coordinates between the Domain and Infrastructure layers.

## Overview

This project implements the use cases and application services that process notification events, manage templates, handle payments, and coordinate the notification delivery workflow. It is built using **.NET 10** and follows the Command/Query pattern (CQRS via MediatR) to implement business workflows.

## Key Components

### Use Cases (Commands/Handlers)

The Application layer implements all major business operations, organized by feature:

#### Authentication
- **LoginHandler**: Authenticates subscribers and manages the login flow.
- **VerifyCodeHandler**: Processes and validates One-Time Password (OTP) codes.
- **RefreshTokenHandler**: Securely handles JWT token renewal.
- **UpdatePasswordHandler**: Manages secure password updates.
- **ForgotPasswordHandler**: Handles the password recovery process.

#### Subscriber Management
- **RegisterSubscriberHandler**: Creates and configures new subscriber accounts.
- **UpdateSubscriberProfileHandler**: Updates subscriber company name and contact information.
- **ManageChannelsHandler**: Configures active notification delivery channels.
- **SetChannelSettingHandler**: Manages fine-grained per-channel configurations.
- **UnsubscribeHandler**: Processes recipient opt-out requests.
- **DeleteAccountHandler**: Handles the permanent deletion of a subscriber account and all associated data.
- **ExportDataHandler**: Exports all subscriber data in a portable format.

#### API Key Management
- **CreateApiKeyHandler**: Generates and hashes new secure API keys.
- **DeleteApiKeyHandler**: Instantly revokes an active API key.
- **UpdateApiKeyScopesHandler**: Updates the allowed scopes for an existing API key.
- **UpdateApiKeyStatusHandler**: Enables or disables an existing API key.

#### Notification Processing
- **ProcessNotificationMessageHandler**: Consumes notification messages from the MassTransit queue and dispatches them via the appropriate channel sender.
- **SendNotificationHandler** (via `NotificationDispatchService`): Orchestrates the rendering and delivery of single notifications.
- **NotificationComplaintHandler**: Processes complaints and feedback from recipients.

#### Workflow Automation
- **CreateWorkflowNotificationHandler**: Creates a new scheduled or event-triggered workflow notification.
- **UpdateWorkflowNotificationHandler**: Updates the configuration of an existing workflow notification.
- **TriggerWorkflowNotificationsHandler**: Processes and dispatches all workflow notifications matching a given event trigger.
- **WorkflowNotificationHandler**: Core handler that executes a single workflow notification (template rendering and delivery).
- **WorkflowQueryAndActionHandlers**: Query handlers for listing/retrieving workflow notifications and command handlers for activating/deactivating/deleting them.

#### Payments
- **CreateExtraCreditPaymentIntentHandler**: Creates a Stripe payment intent for purchasing additional notification credits.
- **ApplySuccessfulExtraCreditPaymentHandler**: Applies credits to the subscriber's account after a successful Stripe payment.
- **GetPaymentHistoryHandler** (via `GetPaymentHistoryQuery`): Retrieves the subscriber's payment transaction history.

#### System Notifications
- **SystemNotificationHandler**: Creates and stores in-app system notifications for a subscriber.
- **ListSystemNotificationsQuery**: Retrieves system notifications with optional read/unread filtering.
- **MarkSystemNotificationAsReadCommand**: Marks a specific system notification as read.

### Services

- **TemplateEngine**: Processes dynamic content using `{{placeholder}}` syntax. Supports nested data (e.g., `{{order.id}}`) and safe handling of missing variables.
- **ApiKeyService**: Handles the secure generation and validation of API keys.
- **NotificationDispatchService**: Orchestrates notification rendering and queuing via MassTransit.
- **SystemNotificationService**: Creates and manages in-app system notifications.
- **StatisticsService**: Provides real-time and historical usage analytics and quota tracking.
- **AutomationSchedulerService**: Schedules and manages automated workflow notifications using cron expressions (Redis-backed).
- **AutomationTriggerService**: Evaluates and dispatches event-triggered workflow notifications.
- **SecurityService**: Provides password hashing and cryptographic utilities.
- **NoOpWorkflowScheduleService**: A no-operation fallback when Redis scheduling is not configured.

### Interfaces

The Application layer defines contracts that are implemented by the Infrastructure layer, following the Dependency Inversion Principle. Interfaces are organized by feature under `Interfaces/`.

**Repositories:**
- `ISubscriberRepository`: Data access for subscriber accounts.
- `ISubscriptionRepository`: Plan management and usage tracking.
- `ITemplateRepository`: Notification template storage and retrieval.
- `IApiKeyRepository`: Secure storage for API key metadata and hashes.
- `IBlacklistRepository`: Management of blocked (forbidden) recipients.
- `INotificationLogRepository`: Access to delivery history logs.
- `IStatsRepository`: Aggregated usage and performance statistics.
- `ISubscriberChannelRepository`: Per-channel configuration storage and retrieval.
- `INotificationComplaintRepository`: Storage and retrieval of notification complaints.
- `IAutomationRepository`: Data access for workflow automation definitions.
- `ISystemNotificationRepository`: Storage and retrieval of in-app system notifications.
- `IPaymentLogRepository`: Payment transaction record persistence and retrieval.

**Services:**
- `INotificationSender`: Abstraction for multi-channel notification delivery (Email, SMS, Push, etc.).
- `INotificationDispatchService`: Contract for queuing and dispatching notifications.
- `ITokenService`: JWT token generation, parsing, and validation.
- `IOtpService`: Secure OTP generation and verification for 2FA login.
- `IEmailService`: Specialized service for complex email operations (e.g., MJML rendering).
- `ISecurityService`: Password hashing and cryptographic utilities.
- `INotificationLogService`: Logging contract for notification delivery outcomes.
- `IStatisticsService`: Contract for retrieving usage analytics.
- `IWorkflowScheduleService`: Abstraction for scheduling workflow notifications (Redis or no-op implementation).
- `ISystemNotificationService`: Contract for creating and managing in-app system notifications.
- `IPaymentService`: Contract for creating payment intents and processing payment events.

### Contracts

- **ProcessNotificationMessage**: MassTransit message contract used to asynchronously pass notification data from the API layer to the background consumer.

### DTOs

Data Transfer Objects are organized by feature under `DTOs/`:
- **Automation**: `WorkflowNotificationDto`
- **Notifications**: `BlacklistEntryDto`, `ChannelConfigurationDto`, `NotificationComplaintDto`, `NotificationLogDto`, `NotificationTemplateDto`, `SystemNotificationDto`
- **Observability**: `StatisticsDto`
- **Payments**: `ExtraCreditPaymentIntentDto`, `PaymentLogDto`
- **Security**: `ApiKeyDto`, `LoginResponseDto`
- **Subscribers**: `ExportDataDto`, `SubscriberDto`

## Notification Workflow

1. **Reception**: A command is received from the API layer (e.g., `SendNotificationCommand`).
2. **Validation**: FluentValidation ensures the input is correct.
3. **Domain Interaction**: The handler interacts with Domain entities (e.g., `Subscriber`, `Subscription`) to enforce business rules.
4. **Processing**: `TemplateEngine` renders the final content using event payload data.
5. **Queuing**: `NotificationDispatchService` publishes a `ProcessNotificationMessage` to RabbitMQ via MassTransit.
6. **Consumption**: `ProcessNotificationMessageHandler` picks up the message and calls `INotificationSender`.
7. **Result**: A structured result (Success/Failure) is returned to the API layer; delivery outcome is logged.

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
- **MediatR**: Implements the Command/Query pattern for use case dispatch.
- **Cronos**: Cron expression parsing for workflow scheduling.
- **Mjml.Net**: MJML email template compilation.
- **Microsoft.Extensions.Logging**: Structured logging across all use cases.

## Testing

Use case handlers are thoroughly tested using **Moq** to isolate dependencies. Test scenarios include:
- Validating correct orchestration of Domain objects.
- Ensuring repository methods are called with expected parameters.
- Verifying error responses for invalid business states (e.g., exceeded limits).
