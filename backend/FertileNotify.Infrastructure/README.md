# FertileNotify.Infrastructure

The Infrastructure layer provides concrete implementations of external concerns such as data persistence, notification delivery, background processing, and third-party integrations.

## Overview

This project contains the technical implementations that support the application's core business logic, including database access, notification senders, background workers, and authentication services.

## Key Components

### Persistence

#### ApplicationDbContext
- Entity Framework Core DbContext for PostgreSQL
- Configures entity mappings and relationships
- Manages database schema through migrations

#### Repositories
- **EfUserRepository**: User data access using Entity Framework
- **EfSubscriptionRepository**: Subscription data management
- **EfTemplateRepository**: Notification template storage and retrieval

### Background Jobs

#### NotificationWorker
- Hosted service that runs continuously in the background
- Picks up queued notifications for processing
- Implements retry logic with exponential backoff
- Handles delivery failures and status updates

#### InMemoryNotificationQueue
- Thread-safe in-memory queue for notification messages
- Supports enqueue/dequeue operations
- Can be replaced with distributed queue (RabbitMQ, Azure Service Bus) for scalability

### Notifications

Implements notification delivery for multiple channels:

#### Email Sender
- **EmailNotificationSender**: SMTP-based email delivery
- Configurable email server settings
- HTML and plain text support

#### SMS Sender
- **SMSNotificationSender**: SMS gateway integration
- Supports various SMS providers
- Message formatting and delivery tracking

#### Console/In-App Sender
- **ConsoleNotificationSender**: Console output for development/testing
- In-app notification storage for dashboard display

### Authentication

- **JwtTokenService**: JWT token generation and validation
- Token-based authentication for API endpoints
- Configurable token expiry and security settings

### Migrations

Contains Entity Framework Core migrations for database schema:
- Initial schema creation
- Entity relationships and constraints
- Seed data for templates and subscriptions

## Database Schema

The infrastructure manages the following entities in PostgreSQL:
- Users
- Subscriptions
- NotificationTemplates
- Notifications
- Events

## Configuration

Infrastructure components are configured through:
- Connection strings in `appsettings.json`
- JWT settings (secret key, issuer, audience)
- SMTP settings for email delivery
- SMS provider credentials
- Background worker intervals

## Background Processing

The NotificationWorker operates in a continuous loop:
1. Dequeue notification from queue
2. Select appropriate sender based on channel (Email/SMS/In-App)
3. Attempt delivery
4. On failure, increment retry count and re-queue (up to max retries)
5. Update notification status (Delivered/Failed)
6. Log delivery results

## Dependencies

- Microsoft.EntityFrameworkCore.Npgsql (PostgreSQL provider)
- Microsoft.Extensions.Hosting (Background workers)
- System.IdentityModel.Tokens.Jwt (JWT authentication)
- FertileNotify.Application (Service interfaces)
- FertileNotify.Domain (Entities and value objects)

## Architecture Role

The Infrastructure layer:
- Implements interfaces defined in the Application layer
- Provides technical capabilities (persistence, messaging, etc.)
- Handles external system integrations
- Remains replaceable without affecting core business logic

This layer is where the application meets the real world - databases, message queues, email servers, and other external services.
