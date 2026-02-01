# FertileNotify.Application

The Application layer contains the business logic orchestration and use case implementations for the Fertile Notify system. This layer defines how the application should behave and coordinates between the Domain and Infrastructure layers.

## Overview

This project implements the use cases and application services that process notification events, manage templates, and coordinate the notification delivery workflow. The Application layer acts as an orchestrator, organizing domain objects to fulfill business use cases.

## Key Components

### Use Cases

Use cases represent specific business operations and follow the Command pattern.

#### ProcessEvent
- **ProcessEventCommand**: Command object containing event data to process
- **ProcessEventHandler**: Orchestrates the notification processing workflow
  - Validates subscriber existence and subscription status
  - Checks subscription limits and enforces usage quotas
  - Retrieves appropriate notification templates for the event type
  - Processes templates with event payload data
  - Creates notification entities in the correct state
  - Queues notifications for asynchronous delivery
  - Handles errors and validation failures gracefully

#### RegisterSubscriber
- **RegisterSubscriberCommand**: Command object for subscriber registration data
- **RegisterSubscriberHandler**: Manages subscriber registration workflow
  - Creates new subscriber accounts with validated data
  - Sets up initial subscription plans
  - Validates email uniqueness and format
  - Hashes passwords securely
  - Initializes preferred communication channels

### Services

Application services provide reusable functionality:

- **TemplateEngine**: Processes notification templates with dynamic content
  - Replaces placeholders with actual values from event payload
  - Supports template variables using double curly brace syntax: `{{variableName}}`
  - Handles nested object properties and complex data structures
  - Generates personalized notification content for each recipient
  - Validates template format and placeholder syntax
  - Example: `"Hello {{userName}}, your order {{orderId}} is confirmed!"`

### Interfaces

Defines service contracts that are implemented by the Infrastructure layer:

**Repositories:**
- `ISubscriberRepository` - Subscriber data access operations
  - SaveAsync, GetByIdAsync, GetByEmailAsync, ExistsAsync, GetExistingIdsAsync
- `ISubscriptionRepository` - Subscription data access and management
  - Create, retrieve, update subscription plans and usage tracking
- `ITemplateRepository` - Template data access and retrieval
  - Get templates by event type and channel

**Services:**
- `INotificationQueue` - Message queue operations for async processing
  - EnqueueAsync, DequeueAsync for managing notification delivery queue
- `INotificationSender` - Notification delivery abstraction
  - SendAsync method for channel-specific notification delivery
- `ITokenService` - JWT token generation and validation
  - GenerateToken for authentication

These interfaces follow the Dependency Inversion Principle, allowing the Application layer to define what it needs without depending on implementation details.

## Workflow

The typical notification processing workflow:

1. **Event Reception**: Receive notification event from API layer via ProcessEventHandler
2. **Subscriber Validation**: Verify subscriber exists and retrieve subscription details
3. **Subscription Validation**: Check subscription plan status and usage limits
4. **Limit Enforcement**: Ensure subscriber hasn't exceeded notification quota
5. **Template Retrieval**: Load appropriate template based on event type and channel
6. **Template Processing**: Use TemplateEngine to populate placeholders with event data
7. **Notification Creation**: Create Notification domain entity with processed content
8. **Queue Management**: Add notification to processing queue for async delivery
9. **Usage Tracking**: Increment subscription usage counter
10. **Background Processing**: Infrastructure picks up from queue for actual delivery

## Template Engine

The TemplateEngine supports dynamic content replacement with flexible placeholder syntax:

### Basic Usage
```csharp
Template: "Hello {{userName}}, welcome to {{companyName}}!"
Payload: { "userName": "Sarah", "companyName": "Acme Corp" }
Result: "Hello Sarah, welcome to Acme Corp!"
```

### Complex Scenarios
```csharp
Template: "Your appointment on {{appointmentDate}} at {{appointmentTime}} is confirmed."
Payload: { "appointmentDate": "2024-02-15", "appointmentTime": "10:30 AM" }
Result: "Your appointment on 2024-02-15 at 10:30 AM is confirmed."
```

### Multiple Placeholders
```csharp
Template: "Order #{{orderId}} for {{itemName}} ({{quantity}} items) totaling ${{total}}"
Payload: { "orderId": "12345", "itemName": "Widget", "quantity": 3, "total": 29.97 }
Result: "Order #12345 for Widget (3 items) totaling $29.97"
```

## Error Handling

The Application layer handles various error scenarios:

- **Subscriber Not Found**: Returns appropriate error when subscriber doesn't exist
- **Subscription Limit Exceeded**: Prevents notification creation when quota is reached
- **Invalid Event Type**: Validates event types before processing
- **Template Not Found**: Handles missing templates gracefully
- **Queue Failures**: Manages queue operation failures with retry logic
- **Validation Errors**: Returns detailed validation error messages

## Dependencies

**Direct Dependencies:**
- **FertileNotify.Domain** - Domain entities, value objects, enums, and business rules
  - Uses Subscriber, Notification, NotificationTemplate, Subscription entities
  - Applies domain rules and validation

**No Dependencies On:**
- Infrastructure layer (follows dependency inversion)
- API layer (inner layer, no knowledge of outer layers)
- External frameworks beyond .NET base libraries

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
- Use case handlers can be unit tested with mocked repositories
- Template engine has comprehensive unit tests
- Workflow orchestration can be verified without external dependencies
- Interface-based design enables easy mocking

Example test structure:
```csharp
// Arrange
var mockSubscriberRepo = new Mock<ISubscriberRepository>();
var mockQueue = new Mock<INotificationQueue>();
var handler = new ProcessEventHandler(mockSubscriberRepo.Object, mockQueue.Object);

// Act
var result = await handler.Handle(command);

// Assert
result.Should().BeSuccessful();
mockQueue.Verify(q => q.EnqueueAsync(It.IsAny<Notification>()), Times.Once);
```

This layer is the heart of the application's behavior, translating user intentions into domain actions while remaining independent of external frameworks and infrastructure concerns.
