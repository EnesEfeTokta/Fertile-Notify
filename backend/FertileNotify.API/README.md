# FertileNotify.API

The API layer serves as the presentation layer of the Fertile Notify application, providing RESTful endpoints for event ingestion, user management, and authentication.

## Overview

This project contains the web API controllers, request/response models, validators, and middleware components that handle HTTP communication with external systems and clients.

## Key Components

### Controllers

- **AuthController**: Handles user authentication and JWT token generation
- **NotificationsController**: Manages notification event ingestion and processing
- **UsersController**: Handles user registration and management

### Models

Contains Data Transfer Objects (DTOs) for:
- Event submission requests
- User registration requests
- Authentication requests and responses
- API responses

### Validators

FluentValidation validators for:
- Event submission validation
- User registration validation
- Request payload validation

### Middlewares

Custom middleware components for:
- Global error handling
- Request logging
- Authentication/Authorization

## API Endpoints

### Authentication
- `POST /api/auth/login` - Authenticate user and receive JWT token

### Users
- `POST /api/users/register` - Register a new user with subscription plan

### Notifications
- `POST /api/notifications` - Submit notification events for processing

## Configuration

The API is configured through `appsettings.json` and supports:
- Database connection strings (PostgreSQL)
- JWT authentication settings (secret, issuer, audience, expiry)
- Logging configuration (Serilog)
- CORS policies
- Notification provider settings

## Dependencies

- ASP.NET Core 9.0
- FluentValidation.AspNetCore
- Microsoft.EntityFrameworkCore
- Microsoft.AspNetCore.Authentication.JwtBearer
- Serilog (structured logging)

## Running the API

```bash
cd FertileNotify.API
dotnet run
```

The API will start on `http://localhost:5000` (HTTP) and `https://localhost:5001` (HTTPS) by default.

## Architecture Role

The API layer depends on:
- **FertileNotify.Application** - For use case handlers and service interfaces
- **FertileNotify.Domain** - For domain entities and value objects
- **FertileNotify.Infrastructure** - For dependency injection configuration

As the outermost layer in Clean Architecture, the API project orchestrates requests and delegates business logic to the Application layer.
