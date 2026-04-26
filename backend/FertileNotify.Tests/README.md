# FertileNotify.Tests

The test project contains unit, integration, and infrastructure tests designed to ensure the reliability, security, and correctness of the Fertile Notify platform.

## Overview

This project uses **xUnit** as its primary testing framework, alongside **Moq** for dependency mocking and **FluentAssertions** for expressive, readable assertions. It is built using **.NET 10** and targets high code coverage for core business logic.

## Test Coverage

### Application Layer Tests

Comprehensive validation of use case orchestration:

#### Use Case Handlers
- **SendNotificationHandlerTests**:
  - Verification of subscriber and subscription validation.
  - Ensuring monthly notification limits are strictly enforced.
  - Validating correct template retrieval and rendering.
  - Mocking external senders and database repositories.
  - Testing error scenarios (e.g., subscriber not found, exceeded quotas).
- **WorkflowNotificationHandlerTests**: Validates the execution of workflow notifications, including template rendering and multi-channel dispatch.
- **SystemNotificationHandlerTests**: Validates in-app system notification creation and retrieval.

### Domain Layer Tests

Pure unit tests for core business components (zero dependencies):

#### Entities
- **SubscriberTests**: Validation of identity and account creation.
- **ForbiddenRecipientTests**: Management of blacklisted emails and phone numbers.
- **AutomationWorkflowTests**: Validation of workflow entity creation, cron scheduling, activation/deactivation, and recipient management.

#### Value Objects
- **EmailAddressTests**: Format validation and immutability checks.
- **PasswordTests**: Secure hashing with BCrypt and verification logic.

#### Domain Concepts
- **EventTypeTests**: Validation of supported event types and catalog lookups.
- **WorkflowCronSchedulingTests**: Cron expression parsing and next-run calculation using `Cronos`.

### Infrastructure Layer Tests

Tests for technical components:

#### Services
- **OtpServiceTests**: Validates OTP generation, storage, verification, and expiry behavior of the in-memory `OtpService`.

### Integration Tests

End-to-end tests using a real test server (`CustomWebApplicationFactory`) backed by an in-memory database:

- **AuthControllerTests**: Validates the full authentication flow (registration → login → OTP verification → token refresh → password change) against the real HTTP pipeline.
- **SubscriberControllerTests**: Validates subscriber profile management, channel configuration, and API key lifecycle through the HTTP API.

#### Test Infrastructure
- **CustomWebApplicationFactory**: Configures a test ASP.NET Core application with in-memory EF Core, fake OTP service, and fake email service for deterministic testing.
- **FakeOtpService**: Provides deterministic OTP codes for integration test scenarios.
- **FakeEmailService**: Stubs out SMTP calls to avoid network dependencies in tests.

## Test Structure

All tests follow the **Arrange-Act-Assert (AAA)** pattern for clarity:

```csharp
[Fact]
public async Task SendNotification_WithExceededLimit_ShouldReturnError()
{
    // Arrange
    var mockSubRepo = new Mock<ISubscriberRepository>();
    var mockSubscriptionRepo = new Mock<ISubscriptionRepository>();
    // ... setup mocks ...
    var handler = new SendNotificationHandler(mockSubRepo.Object, mockSubscriptionRepo.Object, ...);

    // Act
    var result = await handler.Handle(command);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Error.Should().Contain("limit exceeded");
}
```

## Running Tests

### Using dotnet CLI
```bash
cd backend
dotnet test
```

### Specific Filters
```bash
# Run tests for a specific class
dotnet test --filter "FullyQualifiedName~SendNotificationHandlerTests"

# Run only integration tests
dotnet test --filter "FullyQualifiedName~Integration"

# Run with detailed output
dotnet test --logger "console;verbosity=detailed"
```

## Continuous Integration

Tests are integrated into the CI/CD pipeline and must pass before any code is merged into the main branch. High code coverage is maintained for:
- **Domain Logic**: 90%+
- **Application Use Cases**: 85%+
- **Infrastructure Integrations**: 70%+

## Debugging

Tests can be debugged directly from VS Code or Visual Studio by setting breakpoints in the test logic or the production code.

## Technology Stack

- **xUnit**: Main testing framework.
- **Moq**: Versatile mocking of service interfaces and repositories.
- **FluentAssertions**: For readable and maintainable assertion logic.
- **Microsoft.EntityFrameworkCore.InMemory**: Lightweight database testing for unit and integration scenarios.
- **Microsoft.AspNetCore.Mvc.Testing**: ASP.NET Core integration test infrastructure.
