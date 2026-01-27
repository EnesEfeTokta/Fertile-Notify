# FertileNotify.Domain

The Domain layer is the heart of the Fertile Notify application, containing the core business logic, entities, value objects, and business rules. This layer is independent of any external frameworks or technologies.

## Overview

This project defines the fundamental concepts and business rules of the notification system. It represents the ubiquitous language of the domain and encapsulates the essential business knowledge.

## Key Components

### Entities

Domain entities represent core business objects with unique identity:

- **User**: Represents a system user with contact information and preferences
  - Properties: Id, Name, Email, Phone, CreatedAt
  - Manages user identity and contact details

- **Notification**: Represents a notification to be sent to a user
  - Properties: Id, UserId, Channel, Message, Status, CreatedAt, SentAt
  - Tracks notification lifecycle from creation to delivery

- **NotificationTemplate**: Defines reusable notification templates
  - Properties: Id, EventType, Subject, Body, Channel
  - Supports dynamic content through placeholder substitution

- **Subscription**: Manages user subscription plans and limits
  - Properties: Id, UserId, Plan, UsageLimit, CurrentUsage, StartDate, EndDate
  - Enforces subscription-based usage limits

### Value Objects

Immutable objects that describe domain concepts:

- **NotificationChannel**: Represents delivery channels (Email, SMS, InApp)
- **EventType**: Defines types of events that trigger notifications (e.g., UserRegistered, OrderConfirmed)

### Enums

Enumerations defining domain states and types:

- **NotificationStatus**: Pending, Queued, Sent, Delivered, Failed
- **SubscriptionPlan**: Free, Basic, Premium
- **RetryStatus**: Pending, InProgress, MaxRetriesReached
- **Channel**: Email, SMS, InApp

### Events

Domain events that represent significant occurrences:

- Business events triggered when important domain state changes occur
- Enable event-driven architecture and loose coupling
- Allow reaction to domain changes without direct dependencies

### Exceptions

Domain-specific exceptions for business rule violations:

- **InvalidSubscriptionException**: Thrown when subscription limits are exceeded
- **InvalidEventTypeException**: Thrown for unrecognized event types
- **DomainException**: Base class for all domain exceptions

### Rules

Business rules that govern domain behavior:

- Subscription limit validation
- Notification delivery rules
- Template validation rules
- Retry policy rules

## Domain Principles

### Independence
- No dependencies on other layers (Application, Infrastructure, API)
- No dependencies on external frameworks or libraries
- Pure C# with domain logic only

### Ubiquitous Language
- Uses the same terminology as domain experts
- Entity and value object names match business concepts
- Methods and properties express business operations

### Rich Domain Model
- Entities contain both data and behavior
- Business rules are encapsulated within domain objects
- Validation logic lives in the domain

## Business Rules

The domain enforces critical business rules:

1. **Subscription Limits**
   - Free plan: 100 notifications/month
   - Basic plan: 1,000 notifications/month
   - Premium plan: Unlimited notifications

2. **Notification Retry Policy**
   - Maximum 3 retry attempts for failed notifications
   - Exponential backoff between retries
   - Permanent failure after max retries exceeded

3. **Template Requirements**
   - Templates must have valid event types
   - Templates must specify delivery channel
   - Template placeholders must match event payload structure

## Dependencies

- None (by design)
- This layer has no external dependencies, ensuring maximum portability and testability

## Architecture Role

The Domain layer:
- Defines the core business logic and rules
- Is the most stable layer (changes infrequently)
- Is referenced by all other layers
- Contains no infrastructure or framework code
- Represents the business expertise in code

This layer embodies the business knowledge and is protected from external changes, making it the foundation of the Clean Architecture approach.