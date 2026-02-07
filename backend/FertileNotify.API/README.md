# FertileNotify.API

The API layer serves as the presentation layer of the Fertile Notify application, providing RESTful endpoints for notification delivery, subscriber management, and authentication with dual authentication support (JWT + API Keys).

## Overview

This project contains the web API controllers, request/response models, validators, middleware components, and authentication handlers that manage HTTP communication with external systems and clients. It is the entry point for all HTTP requests and coordinates with the Application layer to execute use cases.

## Key Components

### Controllers

- **AuthController**: Handles subscriber authentication with OTP verification and JWT token management
  - Login endpoint with OTP generation
  - OTP verification for secure authentication
  - JWT token generation and refresh
  - Password update functionality
  
- **NotificationsController**: Manages notification sending operations (single and bulk)
  - Send single notification to a subscriber
  - Send bulk notifications to multiple subscribers
  - Validates and queues notifications for asynchronous processing
  
- **SubscribersController**: Handles subscriber registration, profile management, and API key operations
  - Subscriber registration with subscription plans (Free/Pro/Enterprise)
  - Profile retrieval and updates
  - Communication channel preference management (Email, SMS, Console)
  - API key generation, listing, and revocation
  - Subscription plan information

### Models

Contains Data Transfer Objects (DTOs) organized by functionality:

**Authentication Models (4):**
- `LoginRequest`: Email and password for login
- `VerifyOtpRequest`: OTP code verification
- `RefreshTokenRequest`: Token refresh data
- `UpdatePasswordRequest`: Current and new password

**Subscriber Models (3):**
- `RegisterSubscriberRequest`: Email, password, company, phone, subscription plan
- `UpdateChannelsRequest`: Enable/disable notification channels
- `GenerateApiKeyRequest`: Name and description for API key

**Notification Models (4):**
- `SendNotificationRequest`: Single notification with title, message, and channel
- `BulkNotificationRequest`: Multiple notifications with subscriber IDs
- `NotificationDto`: Notification details response
- Standardized response models for success and error cases

### Validators

FluentValidation validators ensure data integrity (10 validators):
- **Authentication Validators**: Login, OTP verification, password update validation
- **Subscriber Validators**: Registration with email format, password strength (min 8 chars, uppercase, lowercase, digit, special char)
- **Notification Validators**: Single and bulk notification request validation with title/message length limits
- **Channel Validators**: Notification channel preference validation
- **API Key Validators**: API key generation request validation

### Middlewares

Custom middleware components for:
- **Global Error Handling**: Catches and formats exceptions as proper HTTP responses
- **Request Logging**: Logs incoming requests and outgoing responses (Serilog integration)

### Authentication

Dual authentication system supporting:
- **JWT Bearer Tokens**: For interactive applications with login flow
  - Generated after OTP verification
  - Includes refresh token mechanism
  - Short-lived access tokens (configurable expiry)
- **API Keys**: For server-to-server integrations
  - Generated via subscriber endpoint
  - Hashed storage for security
  - Can be revoked at any time
  - Includes rate limiting per subscription plan

## API Endpoints

### Authentication
- `POST /api/auth/login` - Login with email/password, generates OTP
- `POST /api/auth/verify-otp` - Verify OTP code and receive JWT token
- `POST /api/auth/refresh-token` - Refresh expired JWT token
- `PUT /api/auth/password` - Update password (requires authentication)

### Subscribers
- `POST /api/subscribers/register` - Register new subscriber with subscription plan (Free/Pro/Enterprise)
- `GET /api/subscribers/profile` - Get authenticated subscriber's profile
- `PUT /api/subscribers/channels` - Update notification channel preferences (Email, SMS, Console)
- `POST /api/subscribers/api-keys` - Generate new API key for server-to-server auth
- `GET /api/subscribers/api-keys` - List all active API keys
- `DELETE /api/subscribers/api-keys/{id}` - Revoke/delete an API key

### Notifications
- `POST /api/notifications/send` - Send single notification to a subscriber
- `POST /api/notifications/bulk` - Send bulk notifications to multiple subscribers

## Configuration

The API is configured through `appsettings.json` and supports:
- **Database Connection**: PostgreSQL connection string
- **JWT Settings**: Secret key (min 32 chars), issuer, audience, token expiry duration (default: 1440 minutes)
- **Logging**: Serilog structured logging with console and file sinks
- **CORS**: Cross-origin resource sharing policies for allowed origins
- **Email Settings**: SMTP host, port, username, password, from address
- **Rate Limiting**: Per-plan rate limits (Free: 50/min, Pro: 100/min, Enterprise: 1000/min)
- **Background Workers**: Notification queue processing intervals and retry policies

Example `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=FertileNotifyDb;Username=postgres;Password=password"
  },
  "JwtSettings": {
    "SecretKey": "your-secret-key-at-least-32-characters-long",
    "Issuer": "FertileNotify",
    "Audience": "FertileNotifyClients",
    "ExpiryInMinutes": 1440
  },
  "EmailSettings": {
    "SmtpHost": "smtp.example.com",
    "SmtpPort": 587,
    "Username": "notifications@example.com",
    "Password": "smtp-password"
  }
}
```

## Dependencies

- **ASP.NET Core 9.0**: Web API framework
- **FluentValidation.AspNetCore**: Request validation library
- **Microsoft.EntityFrameworkCore**: ORM integration with PostgreSQL
- **Microsoft.AspNetCore.Authentication.JwtBearer**: JWT Bearer token authentication
- **Swashbuckle.AspNetCore**: Swagger/OpenAPI documentation
- **Serilog.AspNetCore**: Structured logging with multiple sinks
- **BCrypt.Net-Next**: Password hashing (via Domain layer)

## Running the API

### Development Mode
```bash
cd FertileNotify.API
dotnet run
```

The API will start on:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `https://localhost:5001/swagger`

### Watch Mode (Auto-reload on changes)
```bash
dotnet watch run
```

### Production Mode
```bash
dotnet run --configuration Release
```

### Using Docker
```bash
# From backend directory
cd ..
docker build -t fertile-notify-api -f Dockerfile .
docker run -p 5080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Database=FertileNotifyDb;Username=postgres;Password=password" \
  fertile-notify-api
```

## Subscription Plans & Rate Limiting

The API enforces different limits based on subscription plans:

| Plan | Monthly Notifications | Rate Limit (req/min) | Available Channels | Allowed Events |
|------|----------------------|---------------------|-------------------|----------------|
| **Free** | 100 | 50 | Email | Basic events |
| **Pro** | 1,000 | 100 | Email, SMS | Extended events |
| **Enterprise** | 10,000 | 1,000 | Email, SMS, Console | All events |

Rate limiting returns HTTP 429 (Too Many Requests) when exceeded.

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
