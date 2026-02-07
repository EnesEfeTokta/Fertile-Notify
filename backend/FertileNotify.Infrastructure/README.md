# FertileNotify.Infrastructure

The Infrastructure layer provides concrete implementations of external concerns such as data persistence, notification delivery, background processing, authentication services, and third-party integrations.

## Overview

This project contains the technical implementations that support the application's core business logic. It implements all the interfaces defined in the Application layer, providing the "plumbing" that connects the application to databases, message queues, external services, and other infrastructure concerns.

## Key Components

### Persistence

#### ApplicationDbContext
- Entity Framework Core DbContext for PostgreSQL database
- Configures entity mappings, relationships, and constraints
- Manages database schema through EF Core migrations
- Applies entity configurations for all domain entities
- Handles database transactions and change tracking

#### Entity Configurations
- **SubscriberConfiguration**: Configures Subscriber entity mapping
- **SubscriptionConfiguration**: Configures Subscription entity mapping
- **NotificationTemplateConfiguration**: Configures template storage
- Defines primary keys, foreign keys, indexes, and constraints
- Specifies column types, lengths, and nullability
- Configures value object conversions (EmailAddress, Password, etc.)

#### Repositories
- **EfSubscriberRepository**: Implements ISubscriberRepository
  - Subscriber CRUD operations using Entity Framework Core
  - Email uniqueness validation
  - Password hash storage
  - Async query operations with optimized loading
  - Active channel tracking
  
- **EfSubscriptionRepository**: Implements ISubscriptionRepository
  - Subscription data management and usage tracking
  - Plan validation and limit enforcement
  - Monthly usage increment operations
  - Subscription expiry checking
  
- **EfTemplateRepository**: Implements ITemplateRepository
  - Notification template storage and retrieval
  - Template lookup by event type and channel
  - Template caching for performance optimization

- **EfApiKeyRepository**: Implements IApiKeyRepository
  - API key storage with secure hashing
  - Key lookup and validation
  - Revocation management
  - Subscriber-specific key queries

#### DbSeeder
- Seeds initial data into the database on application startup
- Creates default notification templates for common event types
- Sets up initial subscription plan configurations
- Configures default system settings and test data
- Runs automatically on first application start

### Background Jobs

#### NotificationWorker
- .NET Hosted Service that runs continuously in the background
- Implements IHostedService interface for lifecycle management
- Continuously processes notifications from the queue
- Workflow:
  1. Dequeues notification from InMemoryNotificationQueue
  2. Selects appropriate sender based on channel (Email/SMS/InApp)
  3. Attempts delivery through selected sender
  4. On failure: Increments retry count and re-queues (up to max retries)
  5. Updates notification status (Delivered/Failed)
  6. Logs delivery results and errors
- Configurable processing interval and batch size
- Graceful shutdown handling

#### InMemoryNotificationQueue
- Thread-safe in-memory queue for notification messages
- Implements INotificationQueue interface
- Uses ConcurrentQueue<T> for thread-safe operations
- Supports EnqueueAsync and DequeueAsync operations
- Suitable for:
  - Single-instance deployments
  - Development and testing environments
  - Low to medium notification volumes (<1000/minute)
  
**When to Upgrade:**
Should be replaced with a distributed message queue (RabbitMQ, Azure Service Bus, AWS SQS) when:
- Scaling to multiple API instances (horizontal scaling)
- Processing high volumes (>1000 notifications/minute)
- Requiring guaranteed message delivery and persistence
- Implementing at-least-once delivery semantics
- Deploying in production for high-availability scenarios
- Need for message durability across application restarts

### Notifications

Implements notification delivery for multiple communication channels:

#### EmailNotificationSender
- Implements INotificationSender for Email channel
- SMTP-based email delivery using System.Net.Mail or MailKit
- Features:
  - HTML and plain text email support
  - Attachment handling capability
  - Configurable SMTP server settings (host, port, credentials)
  - TLS/SSL encryption support
  - Template-based email generation
- Configuration in appsettings.json:
  ```json
  {
    "EmailSettings": {
      "SmtpHost": "smtp.example.com",
      "SmtpPort": 587,
      "Username": "notifications@example.com",
      "Password": "encrypted-password",
      "FromAddress": "notifications@example.com",
      "FromName": "Fertile Notify"
    }
  }
  ```

#### SMSNotificationSender
- Implements INotificationSender for SMS channel
- Integrates with SMS gateway providers (Twilio, AWS SNS, etc.)
- Features:
  - Message length validation and truncation
  - International number format support
  - Delivery receipt tracking
  - Cost optimization (message splitting)
- Configurable gateway settings
- Supports multiple SMS providers through abstraction

#### ConsoleNotificationSender
- Implements INotificationSender for InApp/Console channel
- Writes notifications to console output (for development/testing)
- Stores in-app notifications in database for dashboard display
- Useful for:
  - Local development and debugging
  - Automated testing without external dependencies
  - In-app notification history tracking
  - Demonstration and proof-of-concept scenarios

### Authentication

#### JwtTokenService
- Implements ITokenService interface
- JWT (JSON Web Token) generation and validation with refresh token support
- Features:
  - Access token generation with subscriber claims (ID, email, subscription plan)
  - Refresh token generation with cryptographic randomness
  - Configurable access token expiration (default: 1440 minutes)
  - Signing key management with HMAC-SHA256
  - Token validation with issuer and audience verification
  - Claims extraction and parsing
- Configuration:
  ```json
  {
    "JwtSettings": {
      "SecretKey": "your-256-bit-secret-key-here-minimum-32-characters",
      "Issuer": "FertileNotify",
      "Audience": "FertileNotifyClients",
      "ExpiryInMinutes": 1440
    }
  }
  ```
- Used by AuthController for login/OTP authentication endpoints

#### ApiKeyService
- Implements IApiKeyService interface
- API key generation, hashing, and validation
- Features:
  - Secure API key generation with cryptographic randomness (32 bytes)
  - BCrypt hashing for secure storage (work factor: 11)
  - Key validation against hashed values
  - Support for key revocation
  - Key metadata management (name, description)
- Used for server-to-server authentication
- Alternative to JWT for backend integrations

#### OtpService
- Implements IOtpService interface
- One-Time Password (OTP) generation and verification
- Features:
  - 6-digit OTP generation
  - In-memory OTP storage with expiration (5 minutes)
  - Verification with automatic cleanup
  - Thread-safe concurrent access
- Used for two-factor authentication during login

### Migrations

Contains Entity Framework Core migrations for database schema evolution:
- **Initial migrations**: Database schema with Subscribers, Subscriptions, Notifications, NotificationTemplates, and ApiKeys tables
- Schema versioning and rollback capability
- Seed data for default templates, event types, and plans
- Automatic migration application on startup (configurable)

Commands:
```bash
# Create new migration
dotnet ef migrations add MigrationName --project FertileNotify.Infrastructure --startup-project FertileNotify.API

# Update database
dotnet ef database update --project FertileNotify.Infrastructure --startup-project FertileNotify.API

# Rollback migration
dotnet ef database update PreviousMigrationName --project FertileNotify.Infrastructure --startup-project FertileNotify.API

# Generate SQL script
dotnet ef migrations script --project FertileNotify.Infrastructure --startup-project FertileNotify.API
```

## Database Schema

The infrastructure manages the following tables in PostgreSQL:

- **Subscribers**: Subscriber accounts with email, password hash, company, phone, active channels
- **Subscriptions**: Subscription plans (Free/Pro/Enterprise), monthly limits, usage tracking, allowed events
- **NotificationTemplates**: Template definitions for event types with channel-specific content
- **Notifications**: Notification instances with title, message, and creation timestamp
- **ApiKeys**: API key storage with hashed keys, names, descriptions, and revocation status

### Relationships
- Subscriber 1:1 Subscription (one subscriber has one active subscription)
- Subscriber 1:N Notifications (one subscriber can send many notifications)
- Subscriber 1:N ApiKeys (one subscriber can have multiple API keys)
- Notifications reference NotificationTemplates by EventType

## Configuration

Infrastructure components are configured through `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=FertileNotifyDb;Username=postgres;Password=password"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "FertileNotify",
    "Audience": "FertileNotifyClients",
    "ExpiryInMinutes": 1440
  },
  "EmailSettings": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "Username": "user",
    "Password": "pass"
  },
  "BackgroundWorker": {
    "ProcessingIntervalSeconds": 5,
    "MaxRetryAttempts": 3
  }
}
```

## Background Processing Flow

The NotificationWorker operates in a continuous loop:

1. **Dequeue**: Retrieves next notification from the in-memory queue
2. **Select Sender**: Chooses appropriate sender based on notification channel
   - Email → EmailNotificationSender
   - SMS → SMSNotificationSender
   - Console → ConsoleNotificationSender
3. **Attempt Delivery**: Calls SendAsync on the selected sender
4. **Handle Success**: Logs successful delivery
5. **Handle Failure**: 
   - Logs error details
   - Can be extended with retry logic if needed
6. **Update Status**: Updates notification delivery status (if tracking is enabled)
7. **Wait**: Pauses for configured interval before next iteration (default: 5 seconds)

Note: Current implementation uses in-memory queue. For production at scale, consider distributed message queue (RabbitMQ, Azure Service Bus, AWS SQS).

## Retry Strategy

Current implementation: Basic error handling with logging
- Notification delivery failures are logged
- Failed notifications remain in the system for debugging
- Can be extended with retry mechanism if needed

Recommended retry strategy for production:
- **Attempt 1**: Immediate retry
- **Attempt 2**: Wait 30 seconds, then retry
- **Attempt 3**: Wait 2 minutes, then retry (final attempt)
- **After Attempt 3**: Mark as permanently failed or move to dead-letter queue

## Dependencies

**NuGet Packages:**
- **Microsoft.EntityFrameworkCore** (9.0+) - ORM framework
- **Npgsql.EntityFrameworkCore.PostgreSQL** (9.0+) - PostgreSQL provider for EF Core
- **Microsoft.Extensions.Hosting** (9.0+) - Background worker support
- **System.IdentityModel.Tokens.Jwt** (7.0+) - JWT authentication tokens
- **BCrypt.Net-Next** (4.0+) - Password and API key hashing

**Project References:**
- **FertileNotify.Application** - For service interface implementations
- **FertileNotify.Domain** - For entities, value objects, enums, and business rules

## Dependency Injection

All infrastructure services are registered in the DI container:
```csharp
services.AddDbContext<ApplicationDbContext>();
services.AddScoped<ISubscriberRepository, EfSubscriberRepository>();
services.AddScoped<ISubscriptionRepository, EfSubscriptionRepository>();
services.AddScoped<ITemplateRepository, EfTemplateRepository>();
services.AddScoped<IApiKeyRepository, EfApiKeyRepository>();
services.AddSingleton<INotificationQueue, InMemoryNotificationQueue>();
services.AddScoped<INotificationSender, EmailNotificationSender>(); // or factory pattern for channel routing
services.AddScoped<INotificationSender, SMSNotificationSender>();
services.AddScoped<INotificationSender, ConsoleNotificationSender>();
services.AddScoped<ITokenService, JwtTokenService>();
services.AddScoped<IApiKeyService, ApiKeyService>();
services.AddScoped<IOtpService, OtpService>();
services.AddHostedService<NotificationWorker>();
```

## Architecture Role

The Infrastructure layer:
- **Implements Application Interfaces**: Provides concrete implementations for all service contracts
- **Provides Technical Capabilities**: Database access, messaging, external service integration
- **Handles External Systems**: Communicates with databases, email servers, SMS gateways
- **Remains Replaceable**: Can swap implementations without affecting business logic
- **Isolates Technical Details**: Keeps infrastructure concerns separate from domain logic

### Dependency Direction
```
Domain Layer (no dependencies)
     ▲
     │
Application Layer (defines interfaces)
     ▲
     │
Infrastructure Layer (implements interfaces)
```

### Responsibilities
- Database access and data persistence with EF Core
- External service integration (email, SMS gateways)
- Background job processing for async notifications
- Authentication services (JWT tokens, API keys, OTP)
- Configuration management from appsettings.json
- Logging and monitoring infrastructure integration

### Does NOT Contain
- Business rules and logic (Domain layer)
- Use case orchestration (Application layer)
- HTTP/API concerns (API layer)

## Performance Considerations

- **Database Connection Pooling**: EF Core manages connection pool automatically
- **Async Operations**: All I/O operations use async/await for scalability
- **Batch Processing**: Worker can process multiple notifications per cycle
- **Indexing**: Database indexes on frequently queried columns
- **Caching**: Template caching to reduce database queries

## Monitoring and Logging

Infrastructure layer logs:
- Database query execution times
- Notification delivery success/failure
- Background worker activity
- External service call results
- Error stack traces and exception details

Recommended logging levels:
- **Information**: Successful deliveries, worker cycles
- **Warning**: Retry attempts, slow queries
- **Error**: Delivery failures, exceptions, configuration issues

## Testing Infrastructure

- **In-Memory Database**: Use EF Core InMemory provider for repository tests
- **Mock External Services**: Mock SMTP and SMS gateways for testing
- **Integration Tests**: Test database operations with test containers
- **Background Worker Tests**: Verify queue processing and retry logic

This layer is where the application meets the real world—databases, message queues, email servers, and other external services. It provides the foundation that allows the core business logic to function in a production environment.
