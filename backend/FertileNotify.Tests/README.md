# FertileNotify.Tests

The test project contains unit tests and integration tests to ensure the reliability, correctness, and quality of the Fertile Notify notification platform.

## Overview

This project uses **xUnit** as the testing framework along with **FluentAssertions** for readable assertions and **Moq** for mocking dependencies. The test suite validates business logic, use case handlers, services, and integration between components.

## Test Coverage

### Application Layer Tests

#### ProcessEventHandlerTests
Comprehensive tests for the notification event processing use case:

**Core Functionality:**
- Event validation and processing workflow
- Subscription limit enforcement and quota tracking
- Template processing integration with TemplateEngine
- Queue management and notification enqueuing
- Retry mechanism validation for failed deliveries
- Notification status tracking and updates

**Error Scenarios:**
- Subscriber not found handling
- Invalid subscription states
- Exceeded usage limits
- Missing or invalid templates
- Queue operation failures
- Template processing errors

**Edge Cases:**
- Concurrent event processing
- Boundary condition testing (exactly at limit)
- Null and empty payload handling
- Special characters in event data

#### TemplateEngineTests
Thorough tests for the template processing service:

**Template Processing:**
- Placeholder replacement with dynamic values
- Multiple placeholder handling in a single template
- Nested object property access
- Case-sensitive placeholder matching
- Whitespace preservation

**Validation:**
- Template parsing and syntax validation
- Invalid template format detection
- Missing placeholder handling
- Empty template content validation

**Edge Cases:**
- Templates with no placeholders
- Placeholders with special characters
- Escaped braces (non-placeholder)
- Very long templates and content
- Unicode and emoji in templates
- HTML and special character escaping

**Error Handling:**
- Missing values in payload
- Type mismatches
- Null value handling
- Invalid placeholder syntax

## Testing Approach

### Unit Tests
- **Isolation**: Test individual components in complete isolation
- **Mocking**: Mock external dependencies (repositories, senders, queues) using Moq
- **Focus**: Concentrate on business logic correctness
- **Speed**: Fast execution for rapid feedback during development
- **Coverage**: Aim for high code coverage of business-critical paths

### Integration Tests (Planned)
- **Component Interaction**: Test multiple components working together
- **In-Memory Database**: Use EF Core InMemory provider for repository tests
- **End-to-End Flow**: Validate complete notification delivery workflow
- **API Testing**: Test API endpoints with in-memory server
- **Database Operations**: Verify data persistence and retrieval
- **Transaction Handling**: Test transaction boundaries and rollback

## Test Structure

All tests follow the **Arrange-Act-Assert (AAA)** pattern for clarity and consistency:

```csharp
[Fact]
public async Task ProcessEvent_WithValidSubscriber_ShouldEnqueueNotification()
{
    // Arrange - Set up test data and configure mocks
    var mockSubscriberRepo = new Mock<ISubscriberRepository>();
    var mockQueue = new Mock<INotificationQueue>();
    var mockTemplateRepo = new Mock<ITemplateRepository>();
    
    var subscriber = CreateTestSubscriber();
    mockSubscriberRepo
        .Setup(r => r.GetByIdAsync(subscriber.Id))
        .ReturnsAsync(subscriber);
    
    var handler = new ProcessEventHandler(
        mockSubscriberRepo.Object,
        mockQueue.Object,
        mockTemplateRepo.Object
    );
    
    var command = new ProcessEventCommand 
    {
        SubscriberId = subscriber.Id,
        EventType = "order_confirmed",
        Payload = new { orderId = "123", amount = 99.99 }
    };
    
    // Act - Execute the code under test
    var result = await handler.Handle(command);
    
    // Assert - Verify expected outcomes
    result.Should().BeSuccessful();
    mockQueue.Verify(
        q => q.EnqueueAsync(It.IsAny<Notification>()), 
        Times.Once,
        "notification should be queued for delivery"
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
│   │   ├── ProcessEventHandlerTests.cs
│   │   └── RegisterSubscriberHandlerTests.cs (planned)
│   └── Services/
│       └── TemplateEngineTests.cs
├── Domain/ (planned)
│   ├── Entities/
│   │   ├── SubscriberTests.cs
│   │   ├── NotificationTests.cs
│   │   └── SubscriptionTests.cs
│   └── ValueObjects/
│       ├── EmailAddressTests.cs
│       └── PasswordTests.cs
└── Infrastructure/ (planned)
    ├── Repositories/
    │   └── EfSubscriberRepositoryTests.cs
    └── BackgroundJobs/
        └── NotificationWorkerTests.cs
```

## Test Dependencies

**Testing Frameworks:**
- **xUnit** (2.4+) - Main testing framework
  - Supports parallel test execution
  - Extensible with custom attributes
  - Industry-standard for .NET testing

**Assertion Libraries:**
- **FluentAssertions** (6.0+) - Fluent assertion library
  - Readable and expressive assertions
  - Better error messages than standard Assert
  - Supports complex object comparison

**Mocking Frameworks:**
- **Moq** (4.18+) - Mocking framework
  - Create test doubles (mocks, stubs, fakes)
  - Verify method calls and behavior
  - Setup return values and exceptions

**Database Testing:**
- **Microsoft.EntityFrameworkCore.InMemory** (9.0+)
  - In-memory database for repository tests
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

### Domain Layer Tests
- Entity behavior and validation tests
- Value object immutability and equality tests
- Business rule enforcement tests
- Domain event handling tests

### Infrastructure Layer Tests
- Repository integration tests with test database
- Background worker behavior tests
- Notification sender tests (with mocked external services)
- Database migration tests

### API Layer Tests
- Controller action tests
- Input validation tests
- Authentication/authorization tests
- Error handling and status code tests

### End-to-End Tests
- Complete notification flow from API to delivery
- Retry mechanism end-to-end validation
- Multi-channel delivery scenarios
- Subscription limit enforcement

### Performance Tests
- Load testing for queue processing
- Concurrent request handling
- Database query performance
- Memory usage and leak detection

### Security Tests
- JWT token validation and expiry
- SQL injection prevention
- Input sanitization
- Password hashing security

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
- ✅ Template engine functionality
- 🔄 Expanding coverage to domain and infrastructure layers
- 📋 Planning comprehensive integration and E2E tests
