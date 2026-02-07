# FertileNotify.Application

The Application layer contains the business logic orchestration and use case implementations for the Fertile Notify system. This layer defines how the application should behave and coordinates between the Domain and Infrastructure layers.

## Overview

This project implements the use cases and application services that process notification events, manage templates, and coordinate the notification delivery workflow. The Application layer acts as an orchestrator, organizing domain objects to fulfill business use cases.

## Key Components

### Use Cases

Use cases represent specific business operations and follow the Command pattern.

#### ProcessEvent
- **ProcessEventCommand**: Command object containing event data (event type, subscriber ID, payload)
- **ProcessEventHandler**: Orchestrates the notification processing workflow
  - Validates subscriber existence and active subscription status
  - Checks subscription limits and enforces monthly usage quotas
  - Verifies event type is allowed for subscriber's plan
  - Retrieves appropriate notification templates for the event type and channel
  - Processes templates with event payload data using TemplateEngine
  - Creates notification entities with rendered content
  - Routes notification to appropriate sender based on channel (Email/SMS/Console)
  - Increments subscription usage counter
  - Handles errors and validation failures gracefully
  - Returns success/failure result with detailed messages

#### RegisterSubscriber
- **RegisterSubscriberCommand**: Command object for subscriber registration data
- **RegisterSubscriberHandler**: Manages subscriber registration workflow
  - Creates new subscriber accounts with validated data
  - Sets up initial subscription plan (Free/Pro/Enterprise)
  - Validates email uniqueness and format
  - Hashes passwords securely with BCrypt
  - Initializes default active notification channels based on plan
  - Generates initial subscription record with monthly limits
  - Returns subscriber profile DTO on success

### Services

Application services provide reusable functionality:

- **TemplateEngine**: Processes notification templates with dynamic content
  - Replaces placeholders with actual values from event payload
  - Supports template variables using double curly brace syntax: `{{variableName}}`
  - Handles nested object properties and simple data structures
  - Generates personalized notification content for each recipient
  - Validates template format and placeholder syntax
  - Example: `"Hello {{name}}, your order {{orderId}} is ready!"`
  - Returns rendered content with all placeholders replaced

- **ApiKeyService**: Manages API key generation and validation
  - Generates secure random API keys (32 bytes, base64 encoded)
  - Hashes keys using BCrypt for secure storage
  - Validates keys against stored hashes
  - Manages key metadata (name, description, creation date)

- **OtpService**: One-Time Password generation and verification
  - Generates 6-digit random OTP codes
  - Stores OTPs in memory with 5-minute expiration
  - Validates OTP codes during authentication
  - Thread-safe concurrent access
  - Automatic cleanup of expired OTPs

### Interfaces

Defines service contracts that are implemented by the Infrastructure layer:

**Repositories:**
- `ISubscriberRepository` - Subscriber data access operations
  - AddAsync, GetByIdAsync, GetByEmailAsync, UpdateAsync, DeleteAsync
  - Email existence checking
- `ISubscriptionRepository` - Subscription data access and management
  - Create, retrieve, update subscription plans, usage tracking, limit validation
  - Monthly usage increment and reset operations
- `ITemplateRepository` - Template data access and retrieval
  - Get templates by event type and channel
  - List all templates
- `IApiKeyRepository` - API key data access
  - Add, retrieve, revoke API keys
  - Lookup by key hash

**Services:**
- `INotificationQueue` - Message queue operations for async processing
  - EnqueueAsync, DequeueAsync for managing notification delivery queue
- `INotificationSender` - Notification delivery abstraction
  - SendAsync method for channel-specific notification delivery (Email, SMS, Console)
- `ITokenService` - JWT token generation and validation
  - GenerateAccessToken, GenerateRefreshToken, ValidateToken for authentication
- `IApiKeyService` - API key management
  - GenerateApiKey, HashApiKey, ValidateApiKey operations
- `IOtpService` - One-time password operations
  - GenerateOtp, ValidateOtp for two-factor authentication

These interfaces follow the Dependency Inversion Principle, allowing the Application layer to define what it needs without depending on implementation details.

## Workflow

The typical notification processing workflow:

1. **Event Reception**: Receive notification event from API layer via ProcessEventHandler
2. **Subscriber Validation**: Verify subscriber exists and is active
3. **Subscription Validation**: Check subscription plan status and expiry
4. **Limit Enforcement**: Ensure subscriber hasn't exceeded monthly notification quota
5. **Plan Validation**: Verify event type is allowed for subscriber's plan
6. **Channel Validation**: Ensure requested channel is available for subscriber's plan
7. **Template Retrieval**: Load appropriate template based on event type and channel
8. **Template Processing**: Use TemplateEngine to populate `{{placeholders}}` with event data
9. **Notification Creation**: Create Notification domain entity with rendered content
10. **Direct Delivery**: Route notification to appropriate sender (Email/SMS/Console) for immediate delivery
11. **Usage Tracking**: Increment subscription monthly usage counter
12. **Response**: Return success/failure result to API layer

## Template Engine

The TemplateEngine supports dynamic content replacement with flexible placeholder syntax:

### Basic Usage
```csharp
Template: "Hello {{name}}, welcome to {{companyName}}!"
Payload: { "name": "Sarah", "companyName": "Acme Corp" }
Result: "Hello Sarah, welcome to Acme Corp!"
```

### Order Confirmation
```csharp
Template: "Your order #{{orderId}} has been confirmed. Total: ${{total}}"
Payload: { "orderId": "12345", "total": 99.99 }
Result: "Your order #12345 has been confirmed. Total: $99.99"
```

### Appointment Reminder
```csharp
Template: "Reminder: Your appointment on {{date}} at {{time}}"
Payload: { "date": "2024-03-15", "time": "10:30 AM" }
Result: "Reminder: Your appointment on 2024-03-15 at 10:30 AM"
```

## Error Handling

The Application layer handles various error scenarios:

- **Subscriber Not Found**: Returns appropriate error when subscriber doesn't exist in the system
- **Subscription Expired**: Prevents operations when subscription has expired
- **Subscription Limit Exceeded**: Blocks notification creation when monthly quota is reached
- **Invalid Event Type**: Validates event types and rejects unsupported types
- **Unauthorized Event Type**: Blocks event types not allowed for subscriber's plan
- **Template Not Found**: Handles missing templates gracefully with fallback behavior
- **Channel Not Available**: Prevents use of channels not included in subscription plan
- **Validation Errors**: Returns detailed validation error messages to API layer
- **Database Errors**: Handles repository exceptions and connection issues

## Dependencies

**Direct Dependencies:**
- **FertileNotify.Domain** - Domain entities, value objects, enums, and business rules
  - Uses Subscriber, Notification, NotificationTemplate, Subscription, ApiKey entities
  - Applies domain business rules and validation
  - Enforces subscription policies and limits

**No Dependencies On:**
- Infrastructure layer (follows dependency inversion via interfaces)
- API layer (inner layer, no knowledge of outer layers)
- External frameworks beyond .NET base libraries and System.Text.Json

## Design Patterns Used

- **Command Pattern**: Use case commands encapsulate request parameters
- **Handler Pattern**: Handlers execute use case logic
- **Repository Pattern**: Data access through repository interfaces
- **Service Pattern**: Application services for cross-cutting concerns
- **Dependency Inversion**: Depends on abstractions (interfaces), not concrete implementations

## Architecture Role

The Application layer:
- **Implements Use Cases**: Contains application-specific business rules and workflows
- **Defines Service Interfaces**: Specifies contracts that Infrastructure must implement
- **Orchestrates Domain Objects**: Coordinates domain entities to fulfill use cases
- **Remains Framework Independent**: No dependency on UI or database frameworks
- **Mediates Between Layers**: Translates API requests into domain operations

### Layer Dependencies
```
API Layer ───> Application Layer ───> Domain Layer
                     ▲
                     │ (implements interfaces)
                     │
              Infrastructure Layer
```

### Responsibilities
- Use case implementation and business workflow orchestration
- Application service logic (template processing, validation)
- Interface definitions for Infrastructure implementations
- Data transformation between layers (if needed)
- Transaction management and coordination

### Does NOT Contain
- HTTP/REST concerns (API layer responsibility)
- Database queries and data access (Infrastructure layer responsibility)
- Core business rules (Domain layer responsibility)
- UI logic or presentation concerns

## Testing

The Application layer is highly testable:
- Use case handlers can be unit tested with mocked repositories and services
- Template engine has comprehensive unit tests for placeholder replacement
- Workflow orchestration can be verified without external dependencies
- Interface-based design enables easy mocking with frameworks like Moq
- Business logic isolated from infrastructure concerns

Example test structure:
```csharp
// Arrange
var mockSubscriberRepo = new Mock<ISubscriberRepository>();
var mockSubscriptionRepo = new Mock<ISubscriptionRepository>();
var mockTemplateRepo = new Mock<ITemplateRepository>();
var mockNotificationSender = new Mock<INotificationSender>();
var handler = new ProcessEventHandler(
    mockSubscriberRepo.Object, 
    mockSubscriptionRepo.Object,
    mockTemplateRepo.Object,
    mockNotificationSender.Object
);

// Act
var result = await handler.Handle(command);

// Assert
result.Should().BeSuccessful();
mockNotificationSender.Verify(s => s.SendAsync(It.IsAny<Notification>()), Times.Once);
```

This layer is the heart of the application's behavior, translating user intentions into domain actions while remaining independent of external frameworks and infrastructure concerns.
