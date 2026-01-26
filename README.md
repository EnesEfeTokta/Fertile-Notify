# Fertile Notify

<div align="center">

**An event-driven notification platform for centralized multi-channel notification management**

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![.NET](https://img.shields.io/badge/.NET-9.0-purple.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15-blue.svg)](https://www.postgresql.org/)
[![Docker](https://img.shields.io/badge/Docker-Ready-blue.svg)](https://www.docker.com/)
[![Status](https://img.shields.io/badge/Status-Active%20Development-green.svg)](https://github.com/EnesEfeTokta/Fertile-Notify)

</div>

---

## Table of Contents

- [About](#about)
- [Key Features](#key-features)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [Event Flow](#event-flow)
- [API Usage](#api-usage)
- [Configuration](#configuration)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

---

## About

**Fertile Notify** is a robust, scalable notification platform designed to handle event-driven notifications across multiple communication channels. Built with modern software architecture principles, it provides a centralized solution for managing and delivering notifications to users.

The system receives events from external applications, processes notification rules based on user subscriptions and templates, and delivers messages asynchronously using background workers with built-in retry mechanisms and failure handling.

### Why Fertile Notify?

- **Decoupled Architecture**: Clean separation of concerns with layered architecture
- **Event-Driven**: Process notifications asynchronously for better performance
- **Multi-Channel Support**: Send notifications via Email, SMS, and In-App channels
- **Reliable Delivery**: Automatic retry mechanisms with failure tracking
- **Subscription-Based**: Flexible subscription plans with usage limits
- **Template Engine**: Customizable notification templates with dynamic content

---

## Key Features

### Core Capabilities

- **Event-Based Notification Ingestion**
  - RESTful API for receiving events from external systems
  - Support for multiple event types (e.g., user registration, order confirmation, etc.)
  - Event validation and processing

- **Asynchronous Background Processing**
  - Non-blocking notification delivery
  - Queue-based message processing
  - Hosted services for continuous background job execution

- **Multi-Channel Notification Delivery**
  - **Email**: SMTP-based email notifications
  - **SMS**: SMS gateway integration
  - **In-App**: Console/database notifications for in-app messaging

- **Retry & Failure Handling**
  - Automatic retry mechanism for failed deliveries
  - Configurable retry limits
  - Comprehensive failure tracking and logging

- **Subscription Management**
  - Multiple subscription plans (Free, Basic, Premium)
  - Usage limits based on subscription tier
  - Subscription validation before notification delivery

- **Template Engine**
  - Dynamic notification templates
  - Support for placeholders and variable substitution
  - Template management per event type

- **Monitoring & Tracking**
  - Notification status tracking
  - Delivery statistics
  - Admin dashboard for monitoring (planned)

---

## Architecture

Fertile Notify follows **Clean Architecture** principles with a clear separation of concerns:

### Architectural Layers

```
┌─────────────────────────────────────────────┐
│           API Layer (Presentation)          │
│  - Controllers (Event Ingestion)            │
│  - Request/Response Models                  │
│  - API Endpoints                            │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│        Application Layer (Use Cases)        │
│  - Business Logic & Orchestration           │
│  - Use Case Handlers                        │
│  - Service Interfaces                       │
│  - Template Engine                          │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│          Domain Layer (Core Business)       │
│  - Entities (User, Notification, etc.)      │
│  - Value Objects (EventType, Channel)       │
│  - Domain Rules & Validation                │
│  - Enums (NotificationStatus, etc.)         │
└─────────────────┬───────────────────────────┘
                  │
┌─────────────────▼───────────────────────────┐
│    Infrastructure Layer (Implementation)    │
│  - Data Persistence (Repositories)          │
│  - Notification Senders (Email, SMS)        │
│  - Background Workers                       │
│  - External Service Integration             │
└─────────────────────────────────────────────┘
```

For detailed architecture documentation, see [docs/architecture.md](docs/architecture.md).

---

## Tech Stack

### Backend
- **Framework**: ASP.NET Core 9.0
- **Language**: C# 13
- **Architecture**: Clean Architecture / Onion Architecture
- **API**: RESTful API with JSON serialization
- **Validation**: FluentValidation

### Infrastructure
- **Database**: PostgreSQL 15 with Entity Framework Core 9.0
- **ORM**: Entity Framework Core with Migrations
- **Background Jobs**: .NET Hosted Services
- **Messaging Queue**: In-Memory Queue (extensible to RabbitMQ/Azure Service Bus)
- **Containerization**: Docker & Docker Compose

### Notification Channels
- **Email**: SMTP Integration
- **SMS**: SMS Gateway Integration
- **In-App**: Console/Database Notifications

### Development Tools
- **IDE**: Visual Studio / Rider / VS Code
- **Version Control**: Git & GitHub
- **Build System**: .NET CLI / MSBuild
- **Testing**: xUnit, FluentAssertions, Moq
- **Containerization**: Docker & Docker Compose

### Planned Integrations
- **Frontend**: React (planned)
- **Caching**: Redis (planned)
- **Message Broker**: RabbitMQ or Azure Service Bus (planned)

---

## Project Structure

```
Fertile-Notify/
├── backend/
│   ├── FertileNotify.API/              # API Layer - Controllers & Endpoints
│   │   ├── Controllers/                # API Controllers
│   │   ├── Middlewares/                # Custom middlewares
│   │   ├── Models/                     # Request/Response DTOs
│   │   ├── Validators/                 # FluentValidation validators
│   │   └── Program.cs                  # Application entry point
│   │
│   ├── FertileNotify.Application/      # Application Layer - Use Cases
│   │   ├── Interfaces/                 # Service interfaces
│   │   ├── Services/                   # Application services
│   │   └── UseCases/                   # Use case handlers
│   │
│   ├── FertileNotify.Domain/           # Domain Layer - Core Business
│   │   ├── Entities/                   # Domain entities
│   │   ├── Enums/                      # Enumerations
│   │   ├── Events/                     # Domain events
│   │   ├── Exceptions/                 # Domain exceptions
│   │   ├── Rules/                      # Business rules
│   │   └── ValueObjects/               # Value objects
│   │
│   ├── FertileNotify.Infrastructure/   # Infrastructure Layer
│   │   ├── Authentication/             # JWT token service
│   │   ├── BackgroundJobs/             # Background workers
│   │   ├── Migrations/                 # EF Core migrations
│   │   ├── Notifications/              # Notification senders
│   │   └── Persistence/                # EF Core DbContext & repositories
│   │
│   ├── FertileNotify.Tests/            # Unit & Integration Tests
│   │
│   ├── Dockerfile                      # Docker configuration
│   └── FertileNotify.sln               # Solution file
│
├── docs/                               # Documentation
│   └── architecture.md                 # Architecture documentation
│
├── docker-compose.yml                  # Docker Compose configuration
├── .env.example                        # Environment variables template
├── LICENSE                             # GPL-3.0 License
└── README.md                           # This file
```

---

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or higher
- [PostgreSQL 15](https://www.postgresql.org/download/) or higher (optional if using Docker)
- [Docker](https://www.docker.com/get-started) and [Docker Compose](https://docs.docker.com/compose/install/) (optional, recommended)
- IDE: Visual Studio 2022, Rider, or VS Code
- Git

### Installation

#### Option 1: Using Docker (Recommended)

1. **Clone the repository**
   ```bash
   git clone https://github.com/EnesEfeTokta/Fertile-Notify.git
   cd Fertile-Notify
   ```

2. **Configure environment variables**
   ```bash
   cp .env.example .env
   # Edit .env file with your preferred settings
   ```

3. **Start the application with Docker Compose**
   ```bash
   docker-compose up -d
   ```

The API will be available at `http://localhost:5080` (or the port specified in your `.env` file).
The PostgreSQL database will be automatically set up and seeded with initial data.

#### Option 2: Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/EnesEfeTokta/Fertile-Notify.git
   cd Fertile-Notify
   ```

2. **Set up PostgreSQL**
   - Install PostgreSQL 15 or higher
   - Create a database named `FertileNotifyDb`
   - Update connection string in `appsettings.json`

3. **Navigate to the backend directory**
   ```bash
   cd backend
   ```

4. **Restore dependencies**
   ```bash
   dotnet restore
   ```

5. **Apply database migrations**
   ```bash
   cd FertileNotify.API
   dotnet ef database update
   ```

6. **Build the solution**
   ```bash
   dotnet build
   ```

7. **Run the API**
   ```bash
   dotnet run
   ```

The API will start at `http://localhost:5000` (or `https://localhost:5001` for HTTPS).

### Running Tests

```bash
cd backend
dotnet test
```

### Configuration

#### Docker Environment Variables (.env file)

When using Docker Compose, configure the following variables in your `.env` file:

```env
# Database Settings
POSTGRES_USER=your_db_user
POSTGRES_PASSWORD=your_db_password
POSTGRES_DB=FertileNotifyDb
DB_PORT=5432

# API Settings
API_PORT=5080
JWT_SECRET=your_min_30_character_jwt_secret_key
```

#### Application Configuration (appsettings.json)

For local development, configure settings in `FertileNotify.API/appsettings.json`:

- **Connection Strings**: PostgreSQL database connection
- **JWT Settings**: Secret key, issuer, audience, expiry time
- **Notification Providers**: SMTP settings for email, SMS gateway credentials
- **Logging**: Log levels and output configuration

---

## Event Flow

The notification delivery process follows these steps:

```
1. Event Submission
   └─> External system sends event via POST /api/notifications
       
2. Event Validation
   └─> Validate event type, user, and subscription
       
3. Rule Evaluation
   └─> Check subscription limits and notification rules
       
4. Template Processing
   └─> Apply notification template with dynamic data
       
5. Queue Message
   └─> Add notification to delivery queue
       
6. Background Processing
   └─> Worker picks up notification from queue
       
7. Notification Delivery
   └─> Send via appropriate channel (Email/SMS/In-App)
       
8. Retry on Failure
   └─> Automatic retry with exponential backoff
       
9. Status Update
   └─> Mark as Delivered or Failed with tracking
```

---

## API Usage

### Authentication

Before using other endpoints, you need to authenticate and obtain a JWT token.

**Endpoint**: `POST /api/auth/login`

**Request Body**:
```json
{
  "userId": "user-guid-here"
}
```

**Response**:
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

Use the token in subsequent requests:
```
Authorization: Bearer {token}
```

### Register a User

**Endpoint**: `POST /api/users/register`

**Request Body**:
```json
{
  "name": "John Doe",
  "email": "john@example.com",
  "subscriptionPlan": "Premium"
}
```

**Response**:
```json
{
  "userId": "user-123",
  "message": "User registered successfully"
}
```

### Send a Notification

**Endpoint**: `POST /api/notifications`

**Request Body**:
```json
{
  "userId": "user-123",
  "eventType": "user_registered",
  "payload": {
    "userName": "John Doe",
    "email": "john@example.com",
    "registrationDate": "2024-01-15"
  }
}
```

**Response**:
```json
{
  "message": "Event received and queued for processing",
  "eventId": "evt-456"
}
```

### Notification Templates

Templates are defined per event type and can include placeholders:

```csharp
{
  "EventType": "user_registered",
  "Subject": "Welcome {{userName}}!",
  "Body": "Hello {{userName}}, welcome to our platform!"
}
```

### Subscription Plans

- **Free**: Limited to 100 notifications/month
- **Basic**: Up to 1,000 notifications/month
- **Premium**: Unlimited notifications

---

## Contributing

Contributions are welcome! This project is under active development.

### How to Contribute

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Focus

Current development priorities:
- Core event processing and notification delivery ✅ (Completed)
- JWT authentication implementation ✅ (Completed)
- PostgreSQL database integration with EF Core ✅ (Completed)
- Unit and integration tests ✅ (In Progress)
- React-based admin dashboard (Planned)
- Analytics and reporting features (Planned)
- Advanced notification scheduling (Planned)

---

## License

This project is licensed under the **GNU General Public License v3.0** - see the [LICENSE](LICENSE) file for details.

---

## Contact

**Project Maintainer**: Enes Efe Tokta

- GitHub: [@EnesEfeTokta](https://github.com/EnesEfeTokta)
- Repository: [Fertile-Notify](https://github.com/EnesEfeTokta/Fertile-Notify)

---

<div align="center">

**If you find this project useful, please consider giving it a star!**

Made with care by the Fertile Notify team

</div>
