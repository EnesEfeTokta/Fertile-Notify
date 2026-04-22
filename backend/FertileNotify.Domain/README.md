# FertileNotify.Domain

The Domain layer is the core of the Fertile Notify application, containing the business logic, entities, value objects, and fundamental rules. This layer is independent of any external frameworks or technologies, focusing purely on business requirements.

## Overview

This project defines the fundamental concepts and business rules of the notification platform, expressing the ubiquitous language used by the domain. It is built using **.NET 10** and follows Clean Architecture principles by being completely framework-agnostic.

## Key Components

### Entities

Domain entities represent core business objects with unique identities:

- **Subscriber**: Represents a system subscriber with authentication, identity, and profile.
- **Subscription**: Manages subscription tiers (Free, Pro, Enterprise), monthly limits, and usage tracking.
- **Notification**: Represents a single notification message, including its content and state.
- **NotificationTemplate**: Reusable templates supporting dynamic content via `{{placeholder}}` syntax.
- **ApiKey**: Secure API keys for server-to-server authentication, supporting revocation.
- **ForbiddenRecipient**: Management of blacklisted recipients (e.g., opted-out emails or phone numbers).
- **NotificationComplaint**: Records and processes complaints from notification recipients.
- **NotificationLog**: Historical records of notification delivery attempts and results.
- **SubscriberChannelSetting**: Fine-grained control over active notification channels per subscriber.
- **SubscriberDailyStats**: Usage statistics tracked on a daily basis for performance and billing.
- **AutomationWorkflow**: Defines scheduled or event-triggered automated notification workflows with cron expressions.

### Value Objects

Immutable objects describing domain concepts without a unique identity:

- **EmailAddress**: Validates and represents email addresses.
- **Password**: Secure password management using BCrypt hashing.
- **PhoneNumber**: Validates and represents standardized phone numbers.
- **CompanyName**: Enforces business requirements for organization names.
- **NotificationChannel**: Type-safe representation of delivery channels (Email, SMS, Discord, etc.).
- **NotificationContent**: Encapsulates the subject and body of a notification message.
- **RefreshToken**: Secure JWT refresh tokens with expiration tracking.

### Enums

- **SubscriptionPlan**: Defines tiers (Free, Pro, Enterprise) with associated limits and features.
- **DeliveryStatus**: Tracks the delivery status of a notification (Pending, Sent, Failed).
- **ComplaintType**: Categorizes the type of a recipient complaint.
- **EventType**: Defines the supported event types for notification triggers.

### Rules

- **SubscriptionChannelPolicy**: Enforces channel access based on the subscriber's plan.
- **SubscriptionEventPolicy**: Restricts allowed event types based on subscription tier.
- **SubscriptionRule**: Enforces monthly notification limits and quota tracking.
- **ChannelPreferenceRule**: Validates and enforces per-channel recipient preferences.
- **NotificationCostPolicy**: Calculates and applies credit costs per notification type.
- **PasswordRule**: Security requirements for subscriber credentials.

## Domain Principles

### Independence
The Domain layer has **zero external dependencies** and no knowledge of other layers (Application, Infrastructure, API). It only relies on the .NET base class library.

### Rich Domain Model
Entities encapsulate both data and behavior, ensuring that business rules are enforced within the domain itself rather than being leaked into other layers.

### Immutability
All value objects are immutable by design, ensuring thread safety and preventing accidental state changes.

## Business Rules Highlights

1. **Subscription Tiers**:
   - **Free**: 100 notifications/month, Email only.
   - **Pro**: 1,000 notifications/month, Email, SMS, Push.
   - **Enterprise**: 10,000+ notifications/month, All channels.
2. **Security**:
   - Passwords must meet minimum complexity requirements and are hashed with BCrypt.
   - API keys are hashed and can be revoked instantly.
3. **Usage Tracking**:
   - Monthly limits are strictly enforced.
   - Daily statistics are tracked for analytics and quota management.

## Testing

Domain entities and value objects are designed for high testability. Unit tests verify:
- Entity creation and invariant validation.
- Business rule enforcement.
- Value object equality and validation logic.

## Dependencies

- **BCrypt.Net-Next**: For secure password hashing.
- This layer has no project references, sitting at the center of the architecture.
