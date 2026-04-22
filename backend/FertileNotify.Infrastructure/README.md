# FertileNotify.Infrastructure

The Infrastructure layer provides technical implementations for data persistence, notification delivery, asynchronous processing, and third-party integrations. It implements the interfaces defined in the Application layer, providing the "plumbing" that allows the application to function in a production environment.

## Overview

This project contains concrete implementations of external concerns such as database access, message brokers, caching, and notification delivery services. It is built using **.NET 10** and emphasizes performance, reliability, and scalability.

## Key Components

### Persistence

#### ApplicationDbContext
- **Entity Framework Core 10** for PostgreSQL.
- Mappings for all Domain entities, including `Subscriber`, `Subscription`, `NotificationLog`, and `ForbiddenRecipient`.
- Secure value conversions for value objects like `EmailAddress` and `Password`.
- Automated migrations and data seeding via `DbSeeder`.

#### Repositories
- **EfSubscriberRepository**: Subscriber account and preference management.
- **EfSubscriptionRepository**: Plan tracking and monthly quota enforcement.
- **EfTemplateRepository**: High-performance template retrieval.
- **EfApiKeyRepository**: Secure API key hashing and revocation logic.
- **EfBlacklistRepository**: Management of blocked recipients.
- **EfNotificationLogRepository**: Historical delivery tracking.
- **EfStatsRepository**: Performance and usage analytics.
- **EfSubscriberChannelRepository**: Per-channel configuration storage and retrieval.
- **EfINotificationComplaintRepository**: Complaint record persistence and retrieval.
- **EfAutomationRepository**: Workflow automation definition storage.
- **CachedSubscriptionRepository**: Redis-backed caching layer for subscription data to reduce database load.

#### Redis Caching
- Distributed caching using **StackExchange.Redis**.
- Caching for frequently accessed resources (e.g., templates, subscriber channel settings) to reduce database load.

### Asynchronous Processing

#### MassTransit & RabbitMQ
- **Message Broker Integration**: Uses MassTransit for robust, message-driven communication.
- **NotificationConsumer**: A background consumer that processes asynchronous notification messages from the queue.
- **Retry Strategy**: Built-in exponential backoff and dead-letter queue support for failed deliveries.
- **Decoupled Workflow**: The API layer enqueues notifications, and consumers handle the heavy lifting.

### Notification Delivery

Concrete implementations of `INotificationSender` for a wide variety of channels:

- **Email**: `EmailNotificationSender` (SMTP / MailKit) with MJML-responsive rendering.
- **SMS**: `SMSNotificationSender` for international messaging.
- **Collaboration**: `DiscordNotificationSender`, `SlackNotificationSender`, `MSTeamsNotificationSender`.
- **Messaging**: `WhatsAppNotificationSender`, `TelegramNotificationSender`.
- **Push Notifications**: `FirebasePushNotificationSender`, `WebPushNotificationSender`.
- **Custom Integrations**: `WebhookNotificationSender` and `ConsoleNotificationSender` (for testing).

### Authentication & Security

- **JwtTokenService**: Generates secure JWT access and refresh tokens.
- **ApiKeyService**: Cryptographically secure API key generation and BCrypt-based hashing.
- **OtpService**: Multi-threaded, in-memory OTP management for secure 2FA login.

### Background Jobs

- **NotificationConsumer**: MassTransit consumer for processing queued notification requests from RabbitMQ.
- **LogRetentionWorker**: A .NET Hosted Service for cleaning up old notification logs and maintaining database performance.
- **AutomationWorker**: A background worker that triggers scheduled workflow notifications based on cron expressions.
- **RedisSchedulerService**: Manages workflow schedule state in Redis for distributed scheduling.
- **RedisWorkflowScheduleService**: Implements `IWorkflowScheduleService` using Redis for persistent, cross-instance workflow scheduling.

## Database Schema

Managed tables in PostgreSQL include:
- `Subscribers`: Accounts and credentials.
- `Subscriptions`: Plan limits and monthly usage.
- `NotificationLogs`: History of delivery attempts.
- `NotificationTemplates`: Reusable content definitions.
- `ApiKeys`: Secure server-to-server credentials.
- `ForbiddenRecipients`: Blacklisted emails and phone numbers.
- `SubscriberDailyStats`: Performance metrics.

## Configuration

Configuration is managed via `appsettings.json` and environment variables:
- `ConnectionStrings`: PostgreSQL connection info.
- `Redis:ConnectionString`: Redis connection string.
- `RabbitMQ`: Host, credentials, and virtual host settings.
- `JwtSettings`: Security keys and token expiration.
- Provider-specific settings (e.g., SMTP, Firebase, Twilio).

## Performance Considerations

- **Async Everywhere**: All I/O operations (DB, Queue, Caching) use async/await for maximum throughput.
- **Caching**: Redis reduces database latency for read-heavy operations.
- **Indexing**: Database indexes are optimized for frequently queried columns like `SubscriberId` and `CreatedAt`.
- **Batch Processing**: Background consumers handle high volumes with efficient resource usage.

## Dependencies

- **Microsoft.EntityFrameworkCore.PostgreSQL (Npgsql)**: High-performance ORM provider.
- **MassTransit.RabbitMQ**: Enterprise-grade service bus.
- **StackExchange.Redis**: Fast distributed caching and workflow scheduling.
- **MailKit / MimeKit**: Advanced email delivery and MIME construction.
- **FirebaseAdmin**: Push notification support for mobile devices.
- **WebPush**: Web Push notification support via VAPID.
- **System.IdentityModel.Tokens.Jwt**: JWT generation and parsing.
- **Scrutor**: Assembly scanning for automatic dependency injection registration.

## Architecture Role

The Infrastructure layer:
- **Implements Contracts**: Provides the actual logic for all Application layer interfaces.
- **Handles Technical Details**: Isolates technical complexities (e.g., SMTP protocols, SQL queries) from business logic.
- **Ensures Reliability**: Implements retries, error handling, and message persistence.
- **Optimizes Performance**: Manages caching and efficient data access patterns.
