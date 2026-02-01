# FertileNotify.API

The API layer serves as the presentation layer of the Fertile Notify application, providing RESTful endpoints for event ingestion, subscriber management, and authentication.

## Overview

This project contains the web API controllers, request/response models, validators, and middleware components that handle HTTP communication with external systems and clients. It is the entry point for all HTTP requests and coordinates with the Application layer to execute use cases.

## Key Components

### Controllers

- **AuthController**: Handles subscriber authentication and JWT token generation
  - Login endpoint for obtaining JWT tokens
  - Token-based authentication for API access
  
- **NotificationsController**: Manages notification event ingestion and processing
  - Receives events from external systems
  - Validates and queues events for asynchronous processing
  
- **SubscribersController**: Handles subscriber registration and profile management
  - Subscriber registration with subscription plans
  - Profile retrieval and updates
  - Contact information management
  - Communication channel preferences
  - Company information updates
  - Password management

### Models

Contains Data Transfer Objects (DTOs) for:
- **Event Submission**: Event requests with type, subscriber ID, and payload
- **Subscriber Registration**: Registration requests with email, password, company, and subscription plan
- **Authentication**: Login requests and JWT token responses
- **Profile Updates**: Contact info, company name, channel preferences, and password updates
- **API Responses**: Standardized response models for success and error cases

### Validators

FluentValidation validators ensure data integrity:
- **Event Validation**: Event type, subscriber ID, and payload structure validation
- **Subscriber Registration**: Email format, password strength, and required fields
- **Request Payload**: Generic payload validation for complex request objects

### Middlewares

Custom middleware components for:
- **Global Error Handling**: Catches and formats exceptions as proper HTTP responses
- **Request Logging**: Logs incoming requests and outgoing responses
- **Authentication/Authorization**: JWT token validation and authorization checks

## API Endpoints

### Authentication
- `POST /api/auth/login` - Authenticate subscriber and receive JWT token

### Subscribers
- `POST /api/subscribers/register` - Register a new subscriber with subscription plan
- `GET /api/subscribers/profile` - Get authenticated subscriber's profile
- `PUT /api/subscribers/contact` - Update contact information
- `PUT /api/subscribers/company` - Update company name
- `PUT /api/subscribers/channels` - Manage communication channel preferences
- `PUT /api/subscribers/password` - Update password

### Notifications
- `POST /api/notifications` - Submit notification events for processing

## Configuration

The API is configured through `appsettings.json` and supports:
- **Database Connection**: PostgreSQL connection strings
- **JWT Settings**: Secret key, issuer, audience, token expiry duration
- **Logging**: Log levels and structured logging configuration
- **CORS**: Cross-origin resource sharing policies
- **Notification Providers**: SMTP settings for email, SMS gateway credentials
- **Background Workers**: Queue processing intervals and retry policies

## Dependencies

- **ASP.NET Core 9.0**: Web API framework
- **FluentValidation.AspNetCore**: Request validation
- **Microsoft.EntityFrameworkCore**: ORM integration
- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT authentication
- **Serilog**: Structured logging (if configured)

## Running the API

### Development Mode
```bash
cd FertileNotify.API
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`

### Production Mode
```bash
dotnet run --configuration Release
```

### Using Docker
```bash
docker build -t fertile-notify-api .
docker run -p 5080:8080 fertile-notify-api
```

## Architecture Role

The API layer is the outermost layer in Clean Architecture and:

**Depends on:**
- **FertileNotify.Application** - For use case handlers and orchestration logic
- **FertileNotify.Domain** - For domain entities and value objects (indirectly)
- **FertileNotify.Infrastructure** - For dependency injection configuration

**Responsibilities:**
- HTTP request/response handling
- Input validation and sanitization
- Authentication and authorization
- Request routing to appropriate use case handlers
- Response formatting and error handling
- API documentation (Swagger/OpenAPI)

**Does NOT contain:**
- Business logic (delegated to Application layer)
- Data access code (handled by Infrastructure layer)
- Domain rules (encapsulated in Domain layer)

As the entry point of the application, the API project orchestrates incoming requests and delegates business logic execution to the Application layer while keeping presentation concerns separate from core business logic.
