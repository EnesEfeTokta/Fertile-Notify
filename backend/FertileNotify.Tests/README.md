# FertileNotify.Tests

The test project contains unit tests and integration tests to ensure the reliability and correctness of the Fertile Notify notification platform.

## Overview

This project uses xUnit as the testing framework along with FluentAssertions for readable assertions and Moq for mocking dependencies.

## Test Coverage

### Application Layer Tests

#### ProcessEventHandlerTests
Tests for the notification event processing use case:
- Event validation and processing
- Subscription limit enforcement
- Template processing integration
- Queue management
- Retry mechanism validation
- Error handling scenarios

#### TemplateEngineTests
Tests for the template processing service:
- Placeholder replacement with dynamic values
- Template parsing and validation
- Edge cases (missing placeholders, invalid templates)
- Multiple placeholder handling
- Special character escaping

## Testing Approach

### Unit Tests
- Test individual components in isolation
- Mock external dependencies (repositories, senders, queues)
- Focus on business logic correctness
- Fast execution for rapid feedback

### Integration Tests (Planned)
- Test multiple components working together
- In-memory database for repository tests
- End-to-end notification flow validation
- API endpoint testing

## Test Structure

Tests follow the Arrange-Act-Assert (AAA) pattern:

```csharp
[Fact]
public void Test_Scenario_ExpectedBehavior()
{
    // Arrange - Set up test data and mocks
    var mockRepository = new Mock<IUserRepository>();
    var handler = new ProcessEventHandler(mockRepository.Object);
    
    // Act - Execute the code under test
    var result = handler.Handle(testEvent);
    
    // Assert - Verify expected outcomes
    result.Should().BeSuccessful();
}
```

## Running Tests

Execute all tests:
```bash
cd backend
dotnet test
```

Run specific test class:
```bash
dotnet test --filter "FullyQualifiedName~ProcessEventHandlerTests"
```

Run tests with detailed output:
```bash
dotnet test --logger "console;verbosity=detailed"
```

## Test Dependencies

- xUnit - Testing framework
- FluentAssertions - Fluent assertion library for readable tests
- Moq - Mocking framework for creating test doubles
- Microsoft.EntityFrameworkCore.InMemory - In-memory database for integration tests

## Code Coverage (Planned)

Integration with code coverage tools:
- Coverlet for .NET code coverage collection
- ReportGenerator for coverage reports
- CI/CD pipeline integration

## Future Test Additions

- Integration tests for API endpoints
- Database integration tests with test containers
- Performance tests for background worker
- Load tests for queue processing
- End-to-end notification delivery tests
- Security and authentication tests

## Best Practices

1. **Isolation**: Each test should be independent and not rely on other tests
2. **Clarity**: Test names should clearly describe what is being tested
3. **Mocking**: Use mocks to isolate the system under test
4. **Assertions**: Use FluentAssertions for expressive and readable assertions
5. **Maintainability**: Keep tests simple and focused on one concern

## Architecture Role

The Tests project:
- Validates the correctness of all layers
- Ensures business rules are enforced
- Prevents regressions when refactoring
- Documents expected behavior through examples
- Builds confidence for production deployments
