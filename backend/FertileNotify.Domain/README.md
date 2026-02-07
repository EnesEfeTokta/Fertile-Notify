# FertileNotify.Domain

The Domain layer is the heart of the Fertile Notify application, containing the core business logic, entities, value objects, and business rules. This layer is independent of any external frameworks or technologies.

## Overview

This project defines the fundamental concepts and business rules of the notification system. It represents the ubiquitous language of the domain and encapsulates the essential business knowledge. The Domain layer is framework-agnostic and contains pure C# business logic.

## Key Components

### Entities

Domain entities represent core business objects with unique identity:

- **Subscriber**: Represents a system subscriber with authentication and notification preferences
  - Properties: Id, Email, PasswordHash, Company, Phone, CreatedAt, ActiveChannels
  - Manages subscriber identity, authentication credentials, and preferred communication channels
  - Encapsulates subscriber-related business rules
  - Tracks which notification channels are active (Email, SMS, Console)

- **Subscription**: Manages subscriber subscription plans, limits, and usage tracking
  - Properties: Id, SubscriberId, Plan, MonthlyLimit, CurrentUsage, AllowedEventTypes, StartDate, EndDate, IsActive
  - Enforces plan-based access control (Free/Pro/Enterprise)
  - Tracks monthly notification usage
  - Defines allowed event types per plan
  - Validates usage against monthly limits

- **Notification**: Represents a notification message to be sent
  - Properties: Id, SubscriberId, Title, Message, CreatedAt
  - Tracks notification content and creation timestamp
  - Associated with a specific subscriber

- **NotificationTemplate**: Defines reusable notification templates
  - Properties: Id, EventType, Subject, Body, Channel
  - Supports dynamic content through `{{placeholder}}` syntax
  - Maps event types to notification content
  - Channel-specific templates (Email, SMS, Console)

- **ApiKey**: Manages API keys for authentication
  - Properties: Id, SubscriberId, Name, KeyHash, Description, CreatedAt, RevokedAt, IsRevoked
  - Secure API key storage with hashing
  - Supports key revocation
  - Includes metadata (name, description)

### Value Objects

Immutable objects that describe domain concepts without unique identity:

- **EmailAddress**: Represents and validates email addresses
  - Ensures proper email format using regex validation
  - Immutable value object for type safety
  - Explicit operator for string conversion
  
- **Password**: Represents hashed passwords with BCrypt
  - Encapsulates password hashing with BCrypt.Net (work factor: 11)
  - Password verification without exposing hash
  - Ensures password security and complexity
  - Hash and Verify methods for authentication
  
- **PhoneNumber**: Represents validated phone numbers
  - Validates phone number format (optional field)
  - Provides standardized phone number representation
  - Immutable value object
  
- **CompanyName**: Represents company/organization names
  - Validates company name requirements (1-200 characters)
  - Immutable value object for business names
  - Ensures non-empty company names
  
- **NotificationChannel**: Represents delivery channels (enum-like)
  - Defines available channels: Email, SMS, Console
  - Type-safe channel representation
  - Used throughout the system for channel selection

- **RefreshToken**: Represents JWT refresh tokens
  - Immutable token with expiration tracking
  - Token generation with cryptographic randomness
  - Validates token expiry

### Enums

Enumerations defining domain states and types:

- **SubscriptionPlan**: Free, Pro, Enterprise
  - Defines available subscription tiers
  - Each tier has associated limits and features:
    - **Free**: 100 notifications/month, Email only
    - **Pro**: 1,000 notifications/month, Email + SMS
    - **Enterprise**: 10,000 notifications/month, All channels

### Events

Domain events represent significant occurrences in the system:

- **EventType**: Extensible event type system
  - Supports custom event types for different notification scenarios
  - Used to map notifications to templates
  - Examples: "user_registered", "order_confirmed", "payment_received"

Domain events enable:
- Event-driven architecture and loose coupling
- Template-based notification generation
- Flexible notification workflows
- Audit trail and tracking capabilities

### Exceptions

Domain-specific exceptions for business rule violations:

- **InvalidSubscriptionException**: Thrown when subscription limits are exceeded or subscription is invalid
- **InvalidEventTypeException**: Thrown for unrecognized or unsupported event types
- **DomainException**: Base class for all domain-specific exceptions

### Rules

Business rules that govern domain behavior:

- **SubscriptionChannelPolicy**: Controls channel access based on subscription plan
  - Free: Email only
  - Pro: Email + SMS
  - Enterprise: Email + SMS + Console
  
- **SubscriptionEventPolicy**: Restricts event types based on subscription plan
  - Free: Basic event types only
  - Pro: Extended event types
  - Enterprise: All event types
  
- **ChannelPreferenceRule**: Limits active notification channels
  - Maximum 3 active channels per subscriber
  - Validates channel activation requests
  
- **PasswordRule**: Enforces password security requirements
  - Minimum 8 characters
  - At least one uppercase letter
  - At least one lowercase letter
  - At least one digit
  - At least one special character
  
- **SubscriptionRule**: Manages subscription validation
  - Checks subscription expiry
  - Validates active subscription status
  - Enforces monthly notification limits

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
   - **Pro Plan**: 1,000 notifications per month
   - **Enterprise Plan**: 10,000 notifications per month
   - Usage is tracked monthly and enforced before notification delivery

2. **Channel Access Control**
   - **Free Plan**: Email channel only
   - **Pro Plan**: Email + SMS channels
   - **Enterprise Plan**: Email + SMS + Console channels
   - Channels enforced at subscription level

3. **Password Security**
   - Minimum 8 characters required
   - Must contain uppercase, lowercase, digit, and special character
   - Passwords hashed using BCrypt with work factor 11
   - Never stored in plain text

4. **Notification Channels**
   - Subscribers can activate up to 3 channels maximum
   - Channel must be allowed by subscription plan
   - At least one channel must be active

5. **Template Requirements**
   - Templates must specify valid event types
   - Templates must specify target delivery channel (Email, SMS, Console)
   - Template placeholders use `{{variableName}}` syntax
   - Subject and body content must be provided

6. **Subscriber Validation**
   - Email addresses must be unique and valid format
   - Passwords must meet security requirements
   - Company name required (1-200 characters)
   - Phone number optional but validated if provided

## Domain Model Design

### Aggregate Roots
- **Subscriber**: Root entity for subscriber-related operations and authentication
- **Subscription**: Root entity for subscription plan management and usage tracking
- **Notification**: Root entity for notification content and delivery
- **ApiKey**: Root entity for API key management

### Entity Relationships
- One Subscriber has one Subscription (1:1)
- One Subscriber can have many Notifications (1:N)
- One Subscriber can have many ApiKeys (1:N)
- Notifications reference NotificationTemplates by EventType
- Subscriptions track usage limits and enforce monthly quotas

## Dependencies

- **BCrypt.Net-Next** (4.0+) - For secure password hashing
- **System.Text.Json** - For JSON serialization (part of .NET)
- This layer has minimal external dependencies beyond .NET base libraries
- Ensures maximum portability, testability, and maintainability
- Can be reused across different application types (Web API, Console, Mobile, Desktop, etc.)

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
- Subscription limit validation and enforcement
- Password hashing with BCrypt and verification
- Email address format validation
- Phone number format validation
- Channel preference rule (max 3 channels)
- Subscription plan access control policies
- API key generation and validation
- Refresh token expiry validation

This layer embodies the business knowledge and is protected from external changes, making it the foundation of the Clean Architecture approach.