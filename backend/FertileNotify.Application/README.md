# FertileNotify.Application

The Application layer contains the business logic orchestration and use case implementations for the Fertile Notify system. This layer defines how the application should behave and coordinates between the domain and infrastructure layers.

## Overview

This project implements the use cases and application services that process notification events, manage templates, and coordinate the notification delivery workflow.

## Key Components

### Use Cases

#### ProcessEvent
- **ProcessEventHandler**: Orchestrates the notification processing workflow
  - Validates user and subscription
  - Checks subscription limits
  - Retrieves and processes notification templates
  - Queues notifications for delivery
  - Handles retry logic for failed deliveries

#### RegisterUser
- **RegisterUserHandler**: Manages user registration
  - Creates new user accounts
  - Sets up initial subscriptions
  - Validates user data

### Services

- **TemplateEngine**: Processes notification templates with dynamic content
  - Replaces placeholders with actual values from event payload
  - Supports template variables like `{{userName}}`, `{{email}}`, etc.
  - Generates personalized notification content

### Interfaces

Defines service contracts for:
- `INotificationQueue` - Message queue operations
- `INotificationSender` - Notification delivery abstraction
- `ITemplateEngine` - Template processing
- `IUserRepository` - User data access
- `ISubscriptionRepository` - Subscription data access
- `ITemplateRepository` - Template data access

## Workflow

1. **Event Reception**: Receive notification event from API layer
2. **Validation**: Validate user, subscription plan, and limits
3. **Template Processing**: Load template and populate with event data
4. **Queue Management**: Add notification to processing queue
5. **Notification Creation**: Create notification entities with proper status
6. **Background Processing**: Coordinate with infrastructure for async delivery

## Template Engine

The TemplateEngine supports dynamic content replacement:

```csharp
Template: "Hello {{userName}}, your appointment on {{appointmentDate}} is confirmed!"
Payload: { "userName": "Sarah", "appointmentDate": "2024-02-15" }
Result: "Hello Sarah, your appointment on 2024-02-15 is confirmed!"
```

## Dependencies

- FertileNotify.Domain - Domain entities, value objects, and business rules
- No dependencies on Infrastructure or API layers (follows Clean Architecture)

## Architecture Role

The Application layer:
- Implements use cases and application-specific business rules
- Defines service interfaces (implemented by Infrastructure)
- Orchestrates domain objects and coordinates workflows
- Remains independent of external frameworks and UI

This layer is the heart of the application's behavior, translating user intentions into domain actions.
