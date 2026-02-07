# FertileNotify.Tests

The test project contains unit tests and integration tests to ensure the reliability, correctness, and quality of the Fertile Notify notification platform.

## Overview

This project uses **xUnit** and **NUnit** as testing frameworks along with **FluentAssertions** for readable assertions and **Moq** for mocking dependencies. The test suite validates business logic, use case handlers, domain entities, value objects, and services.

## Test Coverage

### Application Layer Tests

#### ProcessEventHandlerTests
Comprehensive tests for the notification event processing use case:

**Core Functionality:**
- Event validation and processing workflow
- Subscription limit enforcement and monthly quota tracking
- Subscription plan validation (Free/Pro/Enterprise)
- Template retrieval and processing integration with TemplateEngine
- Direct notification delivery via appropriate sender (Email/SMS/Console)
- Notification creation with rendered content
- Usage counter increment after successful delivery

**Error Scenarios:**
- Subscriber not found handling
- Inactive or expired subscription states
- Exceeded monthly usage limits
- Missing or invalid templates
- Event type not allowed for plan
- Channel not available for plan
- Template processing errors

**Edge Cases:**
- Boundary condition testing (exactly at monthly limit)
- Null and empty payload handling
- Special characters in event data

#### TemplateEngineTests
Thorough tests for the template processing service:

**Template Processing:**
- Placeholder replacement with dynamic values (`{{variableName}}`)
- Multiple placeholder handling in a single template
- Case-sensitive placeholder matching
- Whitespace preservation around placeholders

**Validation:**
- Template parsing and syntax validation
- Missing placeholder handling (leaves placeholder intact)
- Empty template content validation

**Edge Cases:**
- Templates with no placeholders
- Placeholders with special characters
- Very long templates and content
- Unicode characters in templates

**Error Handling:**
- Missing values in payload (placeholder remains)
- Null value handling
- Invalid placeholder syntax

### Domain Layer Tests

#### SubscriberTests
Tests for Subscriber entity behavior:
- Subscriber creation with valid data
- Email validation through EmailAddress value object
- Password hashing and verification
- Active channel management

#### EmailAddressTests
Tests for EmailAddress value object:
- Valid email format validation
- Invalid email rejection
- Immutability of value object
- Equality comparison

#### PasswordTests
Tests for Password value object:
- BCrypt password hashing with work factor 11
- Password verification (correct and incorrect passwords)
- Hash uniqueness (same password produces different hashes)
- Secure password storage

## Testing Approach

### Unit Tests
- **Isolation**: Test individual components in complete isolation
- **Mocking**: Mock external dependencies (repositories, senders, queues) using Moq
- **Focus**: Concentrate on business logic correctness
- **Speed**: Fast execution for rapid feedback during development
- **Coverage**: Aim for high code coverage of business-critical paths

### Integration Tests (Planned)
- **Component Interaction**: Test multiple components working together
- **In-Memory Database**: Use EF Core InMemory provider for repository integration tests
- **End-to-End Flow**: Validate complete notification delivery workflow
- **API Testing**: Test API endpoints with WebApplicationFactory
- **Database Operations**: Verify data persistence and retrieval with real database
- **Transaction Handling**: Test transaction boundaries and rollback scenarios

## Test Structure

All tests follow the **Arrange-Act-Assert (AAA)** pattern for clarity and consistency:

```csharp
[Fact]
public async Task ProcessEvent_WithValidSubscriber_ShouldSendNotification()
{
    // Arrange - Set up test data and configure mocks
    var mockSubscriberRepo = new Mock<ISubscriberRepository>();
    var mockSubscriptionRepo = new Mock<ISubscriptionRepository>();
    var mockTemplateRepo = new Mock<ITemplateRepository>();
    var mockNotificationSender = new Mock<INotificationSender>();
    
    var subscriber = CreateTestSubscriber();
    var subscription = CreateTestSubscription(subscriber.Id, SubscriptionPlan.Pro);
    
    mockSubscriberRepo
        .Setup(r => r.GetByIdAsync(subscriber.Id))
        .ReturnsAsync(subscriber);
    
    mockSubscriptionRepo
        .Setup(r => r.GetBySubscriberIdAsync(subscriber.Id))
        .ReturnsAsync(subscription);
    
    var handler = new ProcessEventHandler(
        mockSubscriberRepo.Object,
        mockSubscriptionRepo.Object,
        mockTemplateRepo.Object,
        mockNotificationSender.Object
    );
    
    var command = new ProcessEventCommand 
    {
        SubscriberId = subscriber.Id,
        EventType = "order_confirmed",
        Payload = new { orderId = "123", total = 99.99 }
    };
    
    // Act - Execute the code under test
    var result = await handler.Handle(command);
    
    // Assert - Verify expected outcomes
    result.Should().BeSuccessful();
    mockNotificationSender.Verify(
        s => s.SendAsync(It.IsAny<Notification>()), 
        Times.Once,
        "notification should be sent immediately"
    );
}
```

## Running Tests

### Execute All Tests
```bash
cd backend
dotnet test
```

### Run Specific Test Class
```bash
dotnet test --filter "FullyQualifiedName~ProcessEventHandlerTests"
dotnet test --filter "FullyQualifiedName~TemplateEngineTests"
```

### Run Specific Test Method
```bash
dotnet test --filter "FullyQualifiedName~ProcessEventHandlerTests.ProcessEvent_WithValidSubscriber_ShouldEnqueueNotification"
```

### Run with Detailed Output
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Run with Code Coverage (Planned)
```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

### Continuous Test Execution (Watch Mode)
```bash
dotnet watch test
```

## Test Organization

Tests are organized by layer and component:

```
FertileNotify.Tests/
├── Application/
│   ├── UseCases/
│   │   └── ProcessEventHandlerTests.cs
│   └── Services/
│       └── TemplateEngineTests.cs
├── Domain/
│   ├── Entities/
│   │   └── SubscriberTests.cs
│   └── ValueObjects/
│       ├── EmailAddressTests.cs
│       └── PasswordTests.cs
└── Infrastructure/ (planned)
    ├── Repositories/
    │   ├── EfSubscriberRepositoryTests.cs
    │   └── EfSubscriptionRepositoryTests.cs
    └── BackgroundJobs/
        └── NotificationWorkerTests.cs
```

## Test Dependencies

**Testing Frameworks:**
- **xUnit** (2.4+) - Main testing framework for Application and some Domain tests
  - Supports parallel test execution
  - Extensible with custom attributes
  - Industry-standard for .NET testing
- **NUnit** (3.13+) - Alternative testing framework used for some Domain tests
  - Test fixture support
  - Rich assertion library
  - Parameterized tests

**Assertion Libraries:**
- **FluentAssertions** (6.0+) - Fluent assertion library
  - Readable and expressive assertions
  - Better error messages than standard Assert
  - Supports complex object comparison

**Mocking Frameworks:**
- **Moq** (4.18+) - Mocking framework for Application layer tests
  - Create test doubles (mocks, stubs, fakes)
  - Verify method calls and behavior
  - Setup return values and exceptions

**Database Testing:**
- **Microsoft.EntityFrameworkCore.InMemory** (9.0+)
  - In-memory database for repository integration tests (planned)
  - No external database required
  - Fast test execution

## Code Coverage (Planned)

Integration with code coverage tools:

**Coverlet**: .NET code coverage library
- Collects coverage during test execution
- Supports multiple output formats (OpenCover, Cobertura, JSON)
- Integration with CI/CD pipelines

**ReportGenerator**: Coverage report visualization
- Generates HTML coverage reports
- Line-by-line coverage visualization
- Branch coverage analysis

**Target Coverage Goals:**
- **Critical Business Logic**: 90%+ coverage
- **Application Layer**: 85%+ coverage
- **Domain Layer**: 90%+ coverage (pure business logic)
- **Infrastructure Layer**: 70%+ coverage (integration code)

## Future Test Additions

### Domain Layer Tests (Planned)
- Subscription entity behavior and plan validation
- Notification entity creation and properties
- Business rule enforcement tests (SubscriptionRule, ChannelPreferenceRule, PasswordRule)
- Domain event handling tests
- API key entity tests

### Infrastructure Layer Tests (Planned)
- Repository integration tests with test database or in-memory provider
- Background worker behavior tests with mock notification queue
- Notification sender tests (Email, SMS, Console with mocked external services)
- Database migration tests
- JWT token service tests
- API key service tests
- OTP service tests

### API Layer Tests (Planned)
- Controller action tests with mocked dependencies
- Input validation tests with FluentValidation
- Authentication/authorization tests (JWT and API Key)
- Error handling and HTTP status code tests
- Rate limiting tests

### End-to-End Tests (Planned)
- Complete notification flow from API request to delivery
- Subscription plan enforcement end-to-end
- Multi-channel delivery scenarios
- Monthly usage limit tracking across requests
- API key and JWT authentication flows

### Performance Tests (Planned)
- Background worker queue processing throughput
- Concurrent request handling with rate limiting
- Database query performance and optimization
- Memory usage and leak detection
- Template rendering performance

### Security Tests (Planned)
- JWT token validation and expiry
- API key hashing and validation
- OTP generation and expiration
- SQL injection prevention in repositories
- Input sanitization in validators
- BCrypt password hashing security

## Test Data Builders (Planned)

Create reusable test data builders for consistent test setup:

```csharp
public class SubscriberBuilder
{
    public static Subscriber CreateDefault() { ... }
    public static Subscriber WithEmail(string email) { ... }
    public static Subscriber WithSubscription(SubscriptionPlan plan) { ... }
}
```

## Best Practices

### Test Independence
- Each test should be completely independent
- Tests should not rely on execution order
- Use fresh test data for each test
- Clean up resources after tests (if needed)

### Clear Naming
- Test names should describe the scenario and expected outcome
- Format: `MethodName_Scenario_ExpectedBehavior`
- Example: `ProcessEvent_WithExceededLimit_ShouldReturnError`

### Focused Tests
- Each test should verify one specific behavior
- Avoid testing multiple scenarios in a single test
- Keep tests simple and easy to understand

### Effective Mocking
- Mock external dependencies and infrastructure
- Don't mock domain objects or value objects
- Verify important interactions with mocks
- Use strict mocks for critical behaviors

### Readable Assertions
- Use FluentAssertions for expressive assertions
- Provide descriptive failure messages
- Assert on multiple properties when needed
- Use specific assertion methods (Should().BeTrue() vs Should().Be(true))

### Maintainability
- Keep tests DRY (Don't Repeat Yourself) with helper methods
- Use test data builders for complex object creation
- Refactor tests when production code changes
- Update tests along with feature development

## Continuous Integration

Tests are executed automatically in CI/CD pipeline:
- Run on every pull request
- Must pass before merging
- Generate coverage reports
- Fail build on test failures

## Debugging Tests

### Visual Studio
- Right-click test method → "Debug Test"
- Set breakpoints in test or production code
- Step through execution with F10/F11

### VS Code
- Use .NET Test Explorer extension
- Configure launch.json for test debugging
- Set breakpoints and debug interactively

### Command Line
```bash
# Run tests with debugging attached (Windows)
dotnet test --logger:"console;verbosity=detailed" --diag:log.txt

# View test output
cat log.txt
```

## Architecture Role

The Tests project:
- **Validates Correctness**: Ensures all layers work as expected
- **Enforces Business Rules**: Verifies domain rules are properly enforced
- **Prevents Regressions**: Catches bugs when refactoring or adding features
- **Documents Behavior**: Serves as living documentation through examples
- **Builds Confidence**: Enables safe refactoring and deployments
- **Supports Maintenance**: Makes it easier to modify code over time

### Testing Strategy by Layer

**Domain Layer (Pure Business Logic):**
- Easy to test - no dependencies
- Focus on business rule validation
- Test entity behavior and invariants

**Application Layer (Use Cases):**
- Mock all dependencies
- Test orchestration logic
- Verify service interface calls

**Infrastructure Layer (External Concerns):**
- Integration tests with real or in-memory implementations
- Mock external services (email, SMS)
- Test database operations

**API Layer (Presentation):**
- Test request/response handling
- Validate input validation
- Check authentication/authorization

## Summary

The test suite is a critical component that ensures the Fertile Notify platform remains reliable, maintainable, and bug-free. By following testing best practices and maintaining high code coverage, we can confidently deploy changes and add new features without fear of breaking existing functionality.

**Current Test Status:**
- ✅ Application layer core use cases (ProcessEventHandler)
- ✅ Template engine functionality with placeholder replacement
- ✅ Domain entity tests (Subscriber)
- ✅ Domain value object tests (EmailAddress, Password)
- 🔄 Expanding coverage to remaining domain entities and rules
- 📋 Planning comprehensive infrastructure integration tests
- 📋 Planning API controller and end-to-end tests
