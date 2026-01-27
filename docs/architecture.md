# Fertile Notify - System Architecture Documentation

## Table of Contents

1. [Overview](#overview)
2. [Architectural Principles](#architectural-principles)
3. [System Architecture](#system-architecture)
4. [Layer Architecture](#layer-architecture)
5. [Component Design](#component-design)
6. [Data Flow and Event Processing](#data-flow-and-event-processing)
7. [Technology Stack](#technology-stack)
8. [Design Patterns](#design-patterns)
9. [Security Architecture](#security-architecture)
10. [Scalability and Performance](#scalability-and-performance)
11. [Deployment Architecture](#deployment-architecture)
12. [Future Enhancements](#future-enhancements)

---

## Overview

Fertile Notify is an enterprise-grade, event-driven notification platform designed to deliver multi-channel notifications at scale. The system implements Clean Architecture principles with a focus on maintainability, testability, and extensibility. It processes notification events asynchronously, applies business rules based on user subscriptions, and delivers messages through multiple channels (Email, SMS, In-App) with built-in retry mechanisms.

### Core Capabilities

- **Event-Driven Processing**: Asynchronous event ingestion and processing with queue-based architecture
- **Multi-Channel Delivery**: Support for Email, SMS, and In-App notification channels
- **Template-Based Content**: Dynamic content generation using customizable templates
- **Subscription Management**: Tiered subscription plans with usage limits and policy enforcement
- **Reliability**: Automatic retry mechanisms with exponential backoff for failed deliveries
- **Observability**: Comprehensive logging and notification status tracking

---

## Architectural Principles

The system adheres to the following core architectural principles:

### 1. Clean Architecture (Onion Architecture)

The codebase is organized into concentric layers with dependencies flowing inward:

```
Domain (Core) → Application → Infrastructure → Presentation (API)
```

- **Dependency Rule**: Inner layers have no knowledge of outer layers
- **Independence**: Business logic is isolated from frameworks and external concerns
- **Testability**: Each layer can be tested independently

### 2. Separation of Concerns

Each layer has a distinct responsibility:

- **Domain Layer**: Encapsulates business entities, rules, and domain logic
- **Application Layer**: Orchestrates use cases and application workflows
- **Infrastructure Layer**: Implements technical capabilities (persistence, messaging, external services)
- **Presentation Layer**: Handles HTTP requests, routing, and API contracts

### 3. Domain-Driven Design (DDD)

- **Entities**: Rich domain models with behavior (User, Subscription, NotificationTemplate)
- **Value Objects**: Immutable, validated types (EmailAddress, PhoneNumber, NotificationChannel)
- **Domain Events**: EventType and EventCatalog for event taxonomy
- **Business Rules**: Encapsulated validation logic (SubscriptionRule, ChannelPreferenceRule)

### 4. SOLID Principles

- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Extension through interfaces and dependency injection
- **Liskov Substitution**: Interface-based design for notification senders
- **Interface Segregation**: Focused interfaces (INotificationSender, IUserRepository)
- **Dependency Inversion**: High-level modules depend on abstractions

---

## System Architecture

### High-Level Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────┐
│                        External Systems                         │
│                    (Client Applications)                        │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             │ HTTPS/REST API
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                        API Gateway Layer                        │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐          │
│  │ Auth         │  │ Notifications│  │ Users        │          │
│  │ Controller   │  │ Controller   │  │ Controller   │          │
│  └──────────────┘  └──────────────┘  └──────────────┘          │
│         │                  │                  │                 │
│         └──────────────────┴──────────────────┘                 │
└────────────────────────────┬────────────────────────────────────┘
                             │
                             │ Middleware Layer
                             │ (Authentication, Exception Handling)
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                    Application Services Layer                   │
│  ┌──────────────────┐           ┌──────────────────┐            │
│  │ ProcessEvent     │           │ RegisterUser     │            │
│  │ Handler          │           │ Handler          │            │
│  └──────────────────┘           └──────────────────┘            │
│           │                              │                      │
│           ├──────────────────────────────┤                      │
│           │      Template Engine         │                      │
│           └──────────────────────────────┘                      │
└─────────────────────────────────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                      Domain Layer (Core)                        │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐        │
│  │  User    │  │Subscript-│  │Notifica- │  │Template  │        │
│  │  Entity  │  │ion Entity│  │tion      │  │ Entity   │        │
│  └──────────┘  └──────────┘  └──────────┘  └──────────┘        │
│                                                                  │
│  Business Rules, Value Objects, Domain Events                   │
└─────────────────────────────────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                   Infrastructure Layer                          │
│  ┌──────────────────┐  ┌──────────────────┐                     │
│  │   Data Access    │  │  Notification    │                     │
│  │   (EF Core +     │  │  Senders         │                     │
│  │   PostgreSQL)    │  │  - Email (SMTP)  │                     │
│  │                  │  │  - SMS           │                     │
│  │ - User Repo      │  │  - Console       │                     │
│  │ - Subscription   │  └──────────────────┘                     │
│  │ - Template Repo  │                                           │
│  └──────────────────┘                                           │
│                                                                  │
│  ┌──────────────────┐  ┌──────────────────┐                     │
│  │  Background Jobs │  │  Authentication  │                     │
│  │                  │  │  (JWT)           │                     │
│  │ - NotificationWkr│  │                  │                     │
│  │ - Queue (Memory) │  │ - Token Service  │                     │
│  └──────────────────┘  └──────────────────┘                     │
└─────────────────────────────────────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────────┐
│                      External Services                          │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐             │
│  │   SMTP      │  │  SMS Gateway│  │  PostgreSQL │             │
│  │   Server    │  │             │  │  Database   │             │
│  └─────────────┘  └─────────────┘  └─────────────┘             │
└─────────────────────────────────────────────────────────────────┘
```

---

## Layer Architecture

### 1. API Layer (Presentation)

**Location**: `FertileNotify.API`

**Responsibilities**:
- HTTP request handling and routing
- Request validation using FluentValidation
- Response serialization (JSON)
- Authentication and authorization enforcement
- Exception handling middleware

**Key Components**:

```
FertileNotify.API/
├── Controllers/
│   ├── AuthController.cs           # JWT authentication endpoints
│   ├── NotificationsController.cs  # Event submission endpoints
│   └── UsersController.cs          # User management endpoints
├── Middlewares/
│   └── ExceptionHandlingMiddleware.cs  # Centralized error handling
├── Models/
│   ├── RegisterUserRequest.cs      # User registration DTO
│   └── SendNotificationRequest.cs  # Event submission DTO
├── Validators/
│   └── [FluentValidation Rules]    # Input validation
└── Program.cs                      # Application entry point and DI configuration
```

**Design Decisions**:
- RESTful API design with resource-based routing
- JSON serialization with enum string conversion
- Global exception handling to ensure consistent error responses
- Validation at the API boundary using FluentValidation
- JWT Bearer authentication for secure API access

### 2. Application Layer (Use Cases)

**Location**: `FertileNotify.Application`

**Responsibilities**:
- Orchestration of business workflows
- Use case implementation
- Service interfaces definition
- Template rendering and content generation

**Key Components**:

```
FertileNotify.Application/
├── UseCases/
│   ├── ProcessEvent/
│   │   ├── ProcessEventCommand.cs   # Event processing command
│   │   └── ProcessEventHandler.cs   # Core notification processing logic
│   └── RegisterUser/
│       ├── RegisterUserCommand.cs   # User registration command
│       └── RegisterUserHandler.cs   # User creation logic
├── Services/
│   └── TemplateEngine.cs            # Dynamic content rendering
└── Interfaces/
    ├── INotificationQueue.cs        # Queue abstraction
    ├── INotificationSender.cs       # Sender abstraction
    ├── IUserRepository.cs           # User data access
    ├── ISubscriptionRepository.cs   # Subscription data access
    ├── ITemplateRepository.cs       # Template data access
    └── ITokenService.cs             # JWT token generation
```

**Design Decisions**:
- Command pattern for use case encapsulation
- Handler pattern for processing logic separation
- Interface-based dependency injection for testability
- Template engine supports placeholder replacement (e.g., `{{userName}}`)

### 3. Domain Layer (Core Business Logic)

**Location**: `FertileNotify.Domain`

**Responsibilities**:
- Core business entities and their behavior
- Business rules and validation
- Domain events and value objects
- No dependencies on external layers

**Key Components**:

```
FertileNotify.Domain/
├── Entities/
│   ├── User.cs                      # User aggregate with channel management
│   ├── Subscription.cs              # Subscription with usage tracking
│   ├── Notification.cs              # Notification entity
│   └── NotificationTemplate.cs      # Template configuration
├── ValueObjects/
│   ├── EmailAddress.cs              # Validated email type
│   ├── PhoneNumber.cs               # Validated phone number type
│   └── NotificationChannel.cs       # Channel enumeration
├── Events/
│   ├── EventType.cs                 # Event type definition
│   └── EventCatalog.cs              # Predefined event types
├── Rules/
│   ├── SubscriptionRule.cs          # Subscription validation
│   ├── SubscriptionEventPolicy.cs   # Event-subscription mapping
│   └── ChannelPreferenceRule.cs     # Channel limit enforcement
├── Enums/
│   └── SubscriptionPlan.cs          # Free, Pro, Enterprise
└── Exceptions/
    ├── DomainException.cs           # Base domain exception
    ├── BusinessRuleException.cs     # Business rule violations
    └── NotFoundException.cs         # Entity not found
```

**Design Decisions**:
- **Rich Domain Models**: Entities contain behavior, not just data
  - Example: `User.EnableChannel()` enforces business rules
- **Value Objects**: Immutable types with validation
  - Example: `EmailAddress` validates format on construction
- **Domain Events**: Type-safe event definitions (user_registered, order_confirmed)
- **Business Rules**: Encapsulated validation logic
  - Example: `SubscriptionRule.EnsureCanSendNotification()` checks usage limits

### 4. Infrastructure Layer (Technical Implementation)

**Location**: `FertileNotify.Infrastructure`

**Responsibilities**:
- Database access using Entity Framework Core
- External service integration (SMTP, SMS)
- Background job processing
- JWT token generation
- Cross-cutting concerns

**Key Components**:

```
FertileNotify.Infrastructure/
├── Persistence/
│   ├── ApplicationDbContext.cs           # EF Core DbContext
│   ├── EfUserRepository.cs               # User data access
│   ├── EfSubscriptionRepository.cs       # Subscription data access
│   ├── EfTemplateRepository.cs           # Template data access
│   ├── DbSeeder.cs                       # Initial data seeding
│   ├── Configurations/                   # EF entity configurations
│   └── Migrations/                       # Database migrations
├── Notifications/
│   ├── EmailNotificationSender.cs        # SMTP email delivery
│   ├── SMSNotificationSender.cs          # SMS gateway integration
│   └── ConsoleNotificationSender.cs      # Console/logging output
├── BackgroundJobs/
│   ├── NotificationWorker.cs             # Background service for processing
│   └── InMemoryNotificationQueue.cs      # In-memory queue implementation
└── Authentication/
    └── JwtTokenService.cs                # JWT token generation and validation
```

**Design Decisions**:
- **Entity Framework Core**: Code-first approach with migrations
- **PostgreSQL Database**: Relational database for transactional consistency
- **Repository Pattern**: Abstraction over data access
- **Strategy Pattern**: Multiple notification senders implementing `INotificationSender`
- **Background Service**: Hosted service for continuous queue processing
- **In-Memory Queue**: Simple queue for development (can be replaced with RabbitMQ/Azure Service Bus)

---

## Component Design

### Event Processing Flow

The notification system processes events through a series of coordinated components:

```
┌────────────────┐
│ 1. API Request │  POST /api/notifications
└───────┬────────┘
        │
        ▼
┌────────────────────────┐
│ 2. Controller          │  NotificationsController.SendNotification()
│    - Validates request │
│    - Enqueues command  │
└───────┬────────────────┘
        │
        ▼
┌────────────────────────┐
│ 3. Queue               │  INotificationQueue.EnqueueAsync()
│    - In-memory queue   │
│    - Thread-safe       │
└───────┬────────────────┘
        │
        ▼
┌────────────────────────┐
│ 4. Background Worker   │  NotificationWorker (Hosted Service)
│    - Continuous loop   │
│    - Dequeues commands │
└───────┬────────────────┘
        │
        ▼
┌────────────────────────┐
│ 5. Event Handler       │  ProcessEventHandler.HandleAsync()
│    - Validates user    │
│    - Checks subscription│
│    - Applies template  │
│    - Sends notification│
└───────┬────────────────┘
        │
        ├─────────────────────────────┐
        │                             │
        ▼                             ▼
┌──────────────────┐        ┌──────────────────┐
│ 6a. Email Sender │        │ 6b. SMS Sender   │
│     (SMTP)       │        │  (Gateway API)   │
└──────────────────┘        └──────────────────┘
```

### Subscription Management

The subscription system enforces usage limits and event policies:

```csharp
// Subscription Entity
public class Subscription
{
    public SubscriptionPlan Plan { get; }      // Free, Pro, Enterprise
    public int MonthlyUsage { get; }           // Current month's usage
    
    public bool CanHandle(EventType eventType) 
    {
        // Check if subscription plan supports this event type
        return SubscriptionEventPolicy.IsAllowed(Plan, eventType);
    }
    
    public void EnsureCanSendNotification()
    {
        // Enforce usage limits based on plan
        SubscriptionRule.EnsureCanSendNotification(Plan, MonthlyUsage);
    }
}
```

**Subscription Tiers**:
- **Free**: Limited event types, basic notifications
- **Pro**: Extended event types, higher limits
- **Enterprise**: All event types, unlimited notifications

### Template Engine

Dynamic content generation using placeholder replacement:

```csharp
// Template Example
Subject: "Welcome {{userName}}!"
Body: "Hello {{userName}}, your account {{email}} has been created."

// Runtime Rendering
var parameters = new Dictionary<string, string>
{
    ["userName"] = "John Doe",
    ["email"] = "john@example.com"
};

var rendered = templateEngine.Render(template, parameters);
// Output: "Hello John Doe, your account john@example.com has been created."
```

### Notification Channels

Multi-channel delivery with strategy pattern:

```csharp
public interface INotificationSender
{
    NotificationChannel Channel { get; }
    Task SendAsync(User user, string subject, string body);
}

// Implementations:
// - EmailNotificationSender: SMTP-based email delivery
// - SMSNotificationSender: SMS gateway integration
// - ConsoleNotificationSender: Console output for testing
```

---

## Data Flow and Event Processing

### Notification Delivery Sequence

```
┌──────────────────────────────────────────────────────────────────┐
│                     1. Event Submission                          │
│  Client → POST /api/notifications                                │
│  Body: { userId, eventType, payload }                            │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     2. Request Validation                        │
│  - FluentValidation rules                                        │
│  - Schema validation                                             │
│  - Required fields check                                         │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     3. Command Creation                          │
│  ProcessEventCommand {                                           │
│    UserId: Guid,                                                 │
│    EventType: EventType,                                         │
│    Parameters: Dictionary<string, string>                        │
│  }                                                                │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     4. Queue Enqueue                             │
│  - Thread-safe enqueueing                                        │
│  - Return 202 Accepted immediately                               │
│  - Non-blocking operation                                        │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     5. Background Processing                     │
│  NotificationWorker continuously:                                │
│  - Dequeues commands                                             │
│  - Creates scoped services                                       │
│  - Invokes ProcessEventHandler                                   │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     6. User Lookup                               │
│  - userRepository.GetByIdAsync(userId)                           │
│  - Throw NotFoundException if not found                          │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     7. Subscription Validation                   │
│  - subscriptionRepository.GetByUserIdAsync(userId)               │
│  - subscription.CanHandle(eventType)                             │
│  - subscription.EnsureCanSendNotification()                      │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     8. Template Retrieval                        │
│  - templateRepository.GetByEventTypeAsync(eventType)             │
│  - Fallback to default template if not found                     │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     9. Content Rendering                         │
│  - templateEngine.Render(template.SubjectTemplate, parameters)   │
│  - templateEngine.Render(template.BodyTemplate, parameters)      │
│  - Placeholder replacement ({{key}} → value)                     │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     10. Channel Delivery                         │
│  For each user.ActiveChannels:                                   │
│  - Find matching sender                                          │
│  - sender.SendAsync(user, subject, body)                         │
│  - Log delivery status                                           │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     11. Usage Tracking                           │
│  - subscription.IncreaseUsage()                                  │
│  - subscriptionRepository.SaveAsync(userId, subscription)        │
└──────────────────────────────────────────────────────────────────┘
```

### Error Handling Strategy

```
┌──────────────────┐
│  Operation Start │
└────────┬─────────┘
         │
         ▼
┌─────────────────────────┐
│  Try Block              │
│  - Execute operation    │
└────────┬────────────────┘
         │
    ┌────┴────┐
    │         │
    ▼         ▼
  Success   Failure
    │         │
    │         ▼
    │    ┌──────────────────────────┐
    │    │ Exception Type?          │
    │    └────┬────────┬────────┬───┘
    │         │        │        │
    │         ▼        ▼        ▼
    │    ┌─────┐  ┌─────┐  ┌─────┐
    │    │ Biz │  │ Not │  │ Gen │
    │    │ Rule│  │Found│  │eral│
    │    └──┬──┘  └──┬──┘  └──┬──┘
    │       │        │        │
    │       ▼        ▼        ▼
    │    ┌──────────────────────────┐
    │    │ ExceptionHandling        │
    │    │ Middleware               │
    │    │ - Log error              │
    │    │ - Map to HTTP status     │
    │    │ - Return error response  │
    │    └──────────────────────────┘
    │               │
    ▼               ▼
┌──────────────────────────┐
│  Return to Client        │
│  - 200 OK (success)      │
│  - 400 Bad Request       │
│  - 404 Not Found         │
│  - 500 Internal Error    │
└──────────────────────────┘
```

---

## Technology Stack

### Backend Framework
- **ASP.NET Core 8.0**: Modern, cross-platform web framework
- **C# 12**: Latest language features (primary constructors, collection expressions)
- **.NET CLI / MSBuild**: Build and dependency management

### Data Persistence
- **Entity Framework Core 8.0**: ORM for database access
- **PostgreSQL 15**: Production-grade relational database
- **Npgsql**: PostgreSQL data provider for .NET
- **Code-First Migrations**: Database schema versioning

### Dependency Injection
- **Built-in DI Container**: Microsoft.Extensions.DependencyInjection
- **Service Lifetimes**: Singleton, Scoped, Transient
- **Interface-based registration**: Promotes loose coupling

### Validation
- **FluentValidation**: Expressive, fluent API for validation rules
- **FluentValidation.AspNetCore**: Integration with ASP.NET Core model binding

### Authentication & Security
- **JWT Bearer Tokens**: Stateless authentication
- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT middleware
- **SymmetricSecurityKey**: Token signing and validation

### Background Processing
- **.NET Hosted Services**: Long-running background tasks
- **IHostedService Interface**: Background service abstraction
- **In-Memory Queue**: Thread-safe concurrent queue (Channel-based)

### Testing
- **xUnit**: Unit testing framework
- **Moq**: Mocking framework for dependencies
- **FluentAssertions**: Readable test assertions

### Containerization
- **Docker**: Application containerization
- **Docker Compose**: Multi-container orchestration
- **PostgreSQL Alpine Image**: Lightweight database container

### Development Tools
- **Git & GitHub**: Version control
- **Visual Studio / Rider / VS Code**: IDE support
- **Postman / REST Client**: API testing

---

## Design Patterns

### 1. Repository Pattern

**Purpose**: Abstraction layer between domain logic and data access

```csharp
public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task SaveAsync(User user);
}

// Implementation in Infrastructure layer
public class EfUserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    
    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }
}
```

**Benefits**:
- Decouples business logic from data access technology
- Enables unit testing with mock repositories
- Centralized data access logic

### 2. Strategy Pattern

**Purpose**: Interchangeable notification delivery mechanisms

```csharp
public interface INotificationSender
{
    NotificationChannel Channel { get; }
    Task SendAsync(User user, string subject, string body);
}

// Multiple implementations
- EmailNotificationSender (SMTP)
- SMSNotificationSender (SMS Gateway)
- ConsoleNotificationSender (Console output)
```

**Benefits**:
- Easy to add new channels without modifying existing code
- Each channel implementation is isolated
- Runtime selection of appropriate sender

### 3. Command Pattern

**Purpose**: Encapsulate requests as objects

```csharp
public class ProcessEventCommand
{
    public Guid UserId { get; init; }
    public EventType EventType { get; init; }
    public Dictionary<string, string> Parameters { get; init; }
}

public class ProcessEventHandler
{
    public async Task HandleAsync(ProcessEventCommand command)
    {
        // Process the command
    }
}
```

**Benefits**:
- Separates request from execution
- Commands can be queued, logged, or replayed
- Supports undo/redo if needed

### 4. Dependency Injection Pattern

**Purpose**: Inversion of control for loose coupling

```csharp
// Registration in Program.cs
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<ProcessEventHandler>();

// Usage in handler
public class ProcessEventHandler
{
    private readonly IUserRepository _userRepository;
    
    public ProcessEventHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
}
```

**Benefits**:
- Testable code (inject mocks)
- Loose coupling between components
- Configuration centralized in composition root

### 5. Value Object Pattern

**Purpose**: Immutable, validated domain types

```csharp
public class EmailAddress
{
    public string Value { get; }
    
    public EmailAddress(string email)
    {
        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format");
        
        Value = email;
    }
    
    private static bool IsValidEmail(string email)
    {
        // Validation logic
    }
}
```

**Benefits**:
- Encapsulates validation logic
- Type safety (can't pass string where EmailAddress expected)
- Immutability prevents accidental modifications

### 6. Template Method Pattern

**Purpose**: Define algorithm skeleton, allow subclasses to override steps

```csharp
public abstract class NotificationSenderBase : INotificationSender
{
    public abstract NotificationChannel Channel { get; }
    
    public async Task SendAsync(User user, string subject, string body)
    {
        ValidateUser(user);
        var formattedMessage = FormatMessage(subject, body);
        await DeliverMessage(user, formattedMessage);
        LogDelivery(user, subject);
    }
    
    protected abstract Task DeliverMessage(User user, string message);
    protected virtual string FormatMessage(string subject, string body) 
        => $"{subject}\n\n{body}";
}
```

### 7. Middleware Pattern

**Purpose**: Request/response pipeline processing

```csharp
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }
}
```

**Benefits**:
- Centralized cross-cutting concerns
- Ordered pipeline processing
- Easy to add/remove middleware

---

## Security Architecture

### Authentication Flow

```
┌──────────────────────────────────────────────────────────────────┐
│                     1. User Registration                         │
│  POST /api/users/register                                        │
│  { name, email, password }                                       │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     2. Password Hashing                          │
│  - Hash password (planned: bcrypt/PBKDF2)                        │
│  - Store hashed password only                                    │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     3. Login Request                             │
│  POST /api/auth/login                                            │
│  { email, password }                                             │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     4. Credential Validation                     │
│  - Retrieve user by email                                        │
│  - Verify password hash                                          │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     5. JWT Token Generation                      │
│  JwtTokenService.GenerateToken(user)                             │
│  - Claims: userId, email, roles                                  │
│  - Expiration: configurable (default: 1 hour)                    │
│  - Signed with secret key                                        │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     6. Return Token                              │
│  { token: "eyJhbGci...", expiresIn: 3600 }                       │
└────────────────────────────┬─────────────────────────────────────┘
                             │
                             ▼
┌──────────────────────────────────────────────────────────────────┐
│                     7. Authenticated Requests                    │
│  Authorization: Bearer eyJhbGci...                               │
│  - Middleware validates token                                    │
│  - Extracts claims                                               │
│  - Populates HttpContext.User                                    │
└──────────────────────────────────────────────────────────────────┘
```

### Security Best Practices

1. **JWT Configuration**
   - Tokens signed with strong secret key (minimum 256 bits)
   - Short expiration times (1 hour default)
   - Issuer and audience validation
   - HTTPS-only in production

2. **Input Validation**
   - FluentValidation for all API inputs
   - Value objects for domain-level validation
   - SQL injection prevention via parameterized queries (EF Core)
   - XSS prevention via output encoding

3. **Data Protection**
   - Passwords hashed with secure algorithms (planned)
   - Sensitive data encrypted at rest (database-level)
   - HTTPS for data in transit
   - Connection strings in environment variables

4. **Access Control**
   - Role-based authorization (planned)
   - Subscription-level access control
   - API endpoint protection via `[Authorize]` attribute

5. **Monitoring & Logging**
   - Exception logging without sensitive data
   - Failed authentication attempts tracked
   - Audit trail for critical operations

---

## Scalability and Performance

### Current Architecture Scalability

#### Horizontal Scaling Considerations

```
┌─────────────────────────────────────────────────────────────────┐
│                      Load Balancer (Planned)                    │
└───────────┬──────────────────┬──────────────────┬───────────────┘
            │                  │                  │
            ▼                  ▼                  ▼
   ┌────────────────┐ ┌────────────────┐ ┌────────────────┐
   │  API Instance  │ │  API Instance  │ │  API Instance  │
   │      #1        │ │      #2        │ │      #3        │
   └────────┬───────┘ └────────┬───────┘ └────────┬───────┘
            │                  │                  │
            └──────────────────┴──────────────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │  Shared Message Queue│
                    │  (RabbitMQ / Azure)  │
                    └──────────────────────┘
                               │
                    ┌──────────┴──────────┐
                    │                     │
                    ▼                     ▼
            ┌───────────────┐     ┌───────────────┐
            │   Worker #1   │     │   Worker #2   │
            └───────┬───────┘     └───────┬───────┘
                    │                     │
                    └──────────┬──────────┘
                               ▼
                    ┌──────────────────────┐
                    │  PostgreSQL DB       │
                    │  (Read Replicas)     │
                    └──────────────────────┘
```

**Current Limitations**:
- In-memory queue: Single instance only
- Background worker: One worker per instance

**Scalability Path**:
1. Replace in-memory queue with distributed queue (RabbitMQ, Azure Service Bus)
2. Deploy multiple API instances behind load balancer
3. Deploy dedicated worker instances
4. Add database read replicas for query scaling
5. Implement caching layer (Redis) for frequently accessed data

### Performance Optimizations

#### 1. Asynchronous Processing

```csharp
// Non-blocking API endpoints
public async Task<IActionResult> SendNotification(SendNotificationRequest request)
{
    await _queue.EnqueueAsync(command);
    return Accepted(); // Return immediately without waiting for processing
}
```

**Benefits**:
- API responds immediately (< 100ms)
- Processing happens in background
- Better resource utilization

#### 2. Database Optimization

```csharp
// EF Core Configuration
modelBuilder.Entity<User>()
    .HasIndex(u => u.Email)
    .IsUnique();

// Eager loading to prevent N+1 queries
var users = await _context.Users
    .Include(u => u.ActiveChannels)
    .ToListAsync();
```

**Optimizations**:
- Indexed columns for frequent queries (Email, UserId)
- Eager loading for related entities
- Connection pooling enabled by default
- Compiled queries for hot paths

#### 3. Caching Strategy (Planned)

```
┌─────────────────────┐
│  API Request        │
└──────────┬──────────┘
           │
           ▼
    ┌──────────────┐
    │ Redis Cache  │
    │ Check        │
    └──┬──────┬────┘
       │      │
    Hit│      │Miss
       │      │
       ▼      ▼
    Return  ┌──────────────┐
    Cached  │  Database    │
    Data    │  Query       │
            └──────┬───────┘
                   │
                   ▼
            ┌──────────────┐
            │  Cache       │
            │  Result      │
            └──────────────┘
```

**Cache Candidates**:
- Notification templates (infrequently changed)
- User subscription data
- Event type catalog
- Configuration settings

#### 4. Batch Processing (Future Enhancement)

```csharp
// Process notifications in batches
public async Task ProcessBatch(List<ProcessEventCommand> commands)
{
    var users = await _userRepository.GetByIdsAsync(commands.Select(c => c.UserId));
    var subscriptions = await _subscriptionRepository.GetByUserIdsAsync(users.Select(u => u.Id));
    
    // Process entire batch
    foreach (var command in commands)
    {
        // Use cached lookups
    }
}
```

### Performance Metrics

**Target Performance Goals**:
- API Response Time: < 100ms (95th percentile)
- Event Processing Time: < 5 seconds (end-to-end)
- Email Delivery: < 10 seconds
- SMS Delivery: < 5 seconds
- System Throughput: 1000+ events/second
- Database Query Time: < 50ms (95th percentile)

---

## Deployment Architecture

### Containerized Deployment

The system uses Docker for containerization with multi-container orchestration:

```yaml
# docker-compose.yml
services:
  api:
    build: ./backend
    ports: ["8080:8080"]
    environment:
      - ConnectionStrings__DefaultConnection=Host=db;...
      - JwtSettings__SecretKey=${JWT_SECRET}
    depends_on: [db]
    
  db:
    image: postgres:15-alpine
    volumes: [postgres_data:/var/lib/postgresql/data]
    environment:
      - POSTGRES_USER=${POSTGRES_USER}
      - POSTGRES_PASSWORD=${POSTGRES_PASSWORD}
```

### Environment Configuration

**Development**:
- In-memory queue for simplicity
- Console notification sender for testing
- SQL Server LocalDB or PostgreSQL in Docker
- Verbose logging enabled

**Staging**:
- PostgreSQL database
- SMTP server integration
- Redis cache (planned)
- Distributed tracing enabled

**Production**:
- Kubernetes deployment (planned)
- Managed PostgreSQL (Azure Database, AWS RDS)
- Distributed message queue (RabbitMQ, Azure Service Bus)
- Redis cache cluster
- Application monitoring (Application Insights, Datadog)
- Auto-scaling based on queue depth and CPU usage

### Database Migrations

```bash
# Create new migration
dotnet ef migrations add MigrationName --project FertileNotify.Infrastructure

# Apply migrations
dotnet ef database update --project FertileNotify.API

# Production deployment
# Migrations applied during container startup via DbSeeder
```

### Health Checks (Planned)

```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddCheck<NotificationQueueHealthCheck>("queue")
    .AddCheck<SmtpHealthCheck>("smtp");

app.MapHealthChecks("/health");
```

---

## Future Enhancements

### Short-Term Roadmap

1. **Enhanced Authentication**
   - Password hashing with bcrypt
   - Refresh tokens for extended sessions
   - Role-based access control (Admin, User roles)
   - API key authentication for service-to-service calls

2. **Notification Status Tracking**
   - Delivery confirmation callbacks
   - Read receipts for in-app notifications
   - Failure reason logging
   - Retry attempt tracking

3. **Admin Dashboard**
   - React-based frontend
   - Real-time notification monitoring
   - User and subscription management
   - Analytics and reporting

### Medium-Term Roadmap

4. **Distributed Message Queue**
   - Replace in-memory queue with RabbitMQ or Azure Service Bus
   - Message persistence and durability
   - Dead letter queue for failed messages
   - Priority queuing

5. **Advanced Template Features**
   - Rich HTML templates
   - Template versioning
   - A/B testing support
   - Multi-language support (i18n)

6. **Notification Preferences**
   - User-configurable notification settings
   - Quiet hours/Do Not Disturb
   - Channel preferences per event type
   - Frequency capping

### Long-Term Roadmap

7. **Scalability Enhancements**
   - Kubernetes deployment
   - Auto-scaling workers based on queue depth
   - Read replicas for database
   - Redis caching layer
   - CDN integration for static assets

8. **Advanced Features**
   - Scheduled notifications
   - Recurring notifications
   - Notification groups/batching
   - Webhook support for callbacks
   - GraphQL API option

9. **Observability**
   - Distributed tracing (OpenTelemetry)
   - Application Performance Monitoring (APM)
   - Custom metrics and dashboards
   - Alert thresholds and notifications

10. **Compliance & Security**
    - GDPR compliance features
    - Data retention policies
    - Encryption at rest
    - Audit logging
    - Security scanning integration

---

## Conclusion

Fertile Notify is architected as a production-ready, event-driven notification platform built on Clean Architecture principles. The system demonstrates:

- **Separation of Concerns**: Clear layer boundaries with well-defined responsibilities
- **Scalability**: Asynchronous processing with queue-based architecture
- **Extensibility**: Interface-based design for easy addition of new channels and features
- **Maintainability**: Rich domain models, comprehensive testing, and SOLID principles
- **Reliability**: Error handling, retry mechanisms, and validation at all layers

The architecture supports both current operational needs and future growth, with clear paths for horizontal scaling, feature enhancements, and technology migrations.

---

**Document Version**: 1.0  
**Last Updated**: January 2026  
**Maintainer**: Enes Efe Tokta
