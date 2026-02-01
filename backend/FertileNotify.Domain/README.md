# FertileNotify.Domain

The Domain layer is the heart of the Fertile Notify application, containing the core business logic, entities, value objects, and business rules. This layer is independent of any external frameworks or technologies.

## Overview

This project defines the fundamental concepts and business rules of the notification system. It represents the ubiquitous language of the domain and encapsulates the essential business knowledge. The Domain layer is framework-agnostic and contains pure C# business logic.

## Key Components

### Entities

Domain entities represent core business objects with unique identity:

- **Subscriber**: Represents a system subscriber with contact information and preferences
  - Properties: Id, Email, Password (hashed), Company, PreferredChannels, CreatedAt
  - Manages subscriber identity, authentication credentials, and communication preferences
  - Encapsulates subscriber-related business rules

- **Notification**: Represents a notification to be sent to a subscriber
  - Properties: Id, SubscriberId, Channel, Message, Status, CreatedAt, SentAt, RetryCount
  - Tracks notification lifecycle from creation to delivery
  - Manages retry attempts and delivery status

- **NotificationTemplate**: Defines reusable notification templates
  - Properties: Id, EventType, Subject, Body, Channel
  - Supports dynamic content through placeholder substitution
  - Associates templates with specific event types

- **Subscription**: Manages subscriber subscription plans and usage limits
  - Properties: Id, SubscriberId, Plan, UsageLimit, CurrentUsage, StartDate, EndDate
  - Enforces subscription-based usage limits and quotas
  - Tracks usage and validates against plan limits

### Value Objects

Immutable objects that describe domain concepts without unique identity:

- **EmailAddress**: Represents and validates email addresses
  - Ensures proper email format
  - Immutable value object for type safety
  
- **Password**: Represents hashed passwords
  - Encapsulates password hashing logic
  - Ensures password security requirements
  
- **PhoneNumber**: Represents validated phone numbers
  - Validates phone number format
  - Provides standardized phone number representation
  
- **CompanyName**: Represents company/organization names
  - Validates company name requirements
  - Immutable value object for business names
  
- **NotificationChannel**: Represents delivery channels
  - Defines available channels (Email, SMS, InApp)
  - Type-safe channel representation

### Enums

Enumerations defining domain states and types:

- **SubscriptionPlan**: Free, Basic, Premium
  - Defines available subscription tiers
  - Each tier has associated limits and features

### Events

Domain events represent significant occurrences in the system:

- **SubscriberRegistered**: Triggered when a new subscriber joins the platform
- **NotificationSent**: Fired when a notification is successfully delivered
- **SubscriptionExpired**: Raised when a subscriber's subscription plan expires
- **NotificationFailed**: Emitted when notification delivery fails after max retries

Domain events enable:
- Event-driven architecture and loose coupling
- Reaction to domain changes without direct dependencies
- Audit trail and event sourcing capabilities
- Asynchronous processing of side effects

### Exceptions

Domain-specific exceptions for business rule violations:

- **InvalidSubscriptionException**: Thrown when subscription limits are exceeded or subscription is invalid
- **InvalidEventTypeException**: Thrown for unrecognized or unsupported event types
- **DomainException**: Base class for all domain-specific exceptions

### Rules

Business rules that govern domain behavior:

- **Subscription Limit Validation**: Enforces usage limits based on subscription plan
- **Notification Delivery Rules**: Determines when and how notifications should be sent
- **Template Validation Rules**: Ensures templates are properly formatted and valid
- **Retry Policy Rules**: Manages retry attempts and backoff strategies

## Domain Principles

### Independence
- **Zero External Dependencies**: No dependencies on other layers (Application, Infrastructure, API)
- **Framework Agnostic**: No dependencies on external frameworks or libraries beyond .NET base class library
- **Pure Business Logic**: Contains only domain logic and business rules

### Ubiquitous Language
- **Domain Expert Terminology**: Uses the same terminology as domain experts and stakeholders
- **Consistent Naming**: Entity and value object names match business concepts precisely
- **Expressive Methods**: Method and property names clearly express business operations

### Rich Domain Model
- **Data + Behavior**: Entities contain both data and the behavior that operates on that data
- **Encapsulated Rules**: Business rules are encapsulated within domain objects
- **Self-Validating**: Entities validate their own state and ensure consistency

### Immutability Where Appropriate
- **Value Objects**: All value objects are immutable by design
- **Thread Safety**: Immutable objects provide thread-safe access
- **Predictable State**: Reduces bugs related to unexpected state changes

## Business Rules

The domain enforces critical business rules:

1. **Subscription Limits**
   - **Free Plan**: 100 notifications per month
   - **Basic Plan**: 1,000 notifications per month
   - **Premium Plan**: Unlimited notifications
   - Usage is tracked and enforced before notification delivery

2. **Notification Retry Policy**
   - Maximum 3 retry attempts for failed notification deliveries
   - Exponential backoff between retry attempts
   - Permanent failure status after max retries exceeded
   - Retry count tracked per notification

3. **Template Requirements**
   - Templates must specify valid event types
   - Templates must specify target delivery channel
   - Template placeholders must match event payload structure
   - Subject and body content must be provided

4. **Subscriber Validation**
   - Email addresses must be unique and valid format
   - Passwords must meet security requirements
   - Preferred channels must be valid channel types
   - Company name required for business subscribers

## Domain Model Design

### Aggregate Roots
- **Subscriber**: Root entity for subscriber-related operations
- **Notification**: Root entity for notification lifecycle management
- **Subscription**: Root entity for subscription and usage tracking

### Entity Relationships
- One Subscriber can have one Subscription
- One Subscriber can have many Notifications
- Notifications are associated with NotificationTemplates by EventType
- Subscriptions track usage limits and current usage

## Dependencies

- **None** (by design)
- This layer has zero external dependencies beyond .NET base libraries
- Ensures maximum portability, testability, and maintainability
- Can be reused across different application types (Web API, Console, Mobile, etc.)

## Architecture Role

The Domain layer:
- **Defines Core Business Logic**: Contains all essential business rules and operations
- **Most Stable Layer**: Changes infrequently as business rules are relatively stable
- **Referenced by All Layers**: Application, Infrastructure, and API layers all depend on Domain
- **Framework Independent**: Contains no infrastructure or framework-specific code
- **Represents Business Expertise**: Embodies the business knowledge and domain understanding

### Dependency Direction
```
API Layer ──────┐
                ├──> Application Layer ──> Domain Layer (no dependencies)
Infrastructure ─┘
```

The Domain layer sits at the center and has no knowledge of outer layers, following the Dependency Inversion Principle and Clean Architecture guidelines.

## Testing the Domain

Domain entities and value objects are:
- Easy to unit test (no mocking required)
- Fast to test (no database or external dependencies)
- Reliable (deterministic behavior)
- Self-contained (all logic within domain objects)

Example test scenarios:
- Subscription limit validation
- Password hashing and verification
- Email address format validation
- Notification retry count tracking
- Template placeholder replacement

This layer embodies the business knowledge and is protected from external changes, making it the foundation of the Clean Architecture approach.