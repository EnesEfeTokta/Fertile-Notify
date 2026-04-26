# Fertile Notify

<div align="center">

**An event-driven notification platform for centralized multi-channel notification management**

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)
[![.NET](https://img.shields.io/badge/.NET-10.0-purple.svg)](https://dotnet.microsoft.com/)
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
- **Multi-Channel Support**: Send notifications via Email, SMS, Push, Discord, Slack, MS Teams, WhatsApp, Telegram, and more
- **Reliable Delivery**: Automatic retry mechanisms with failure tracking
- **Subscription-Based**: Flexible subscription plans (Free, Pro, Enterprise) with usage limits and extra-credit top-ups
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
  - **Email**: SMTP-based email notifications with MJML responsive templates
  - **SMS**: SMS gateway integration
  - **Push**: Firebase push notifications for mobile/web
  - **Collaboration**: Discord, Slack, MS Teams
  - **Messaging**: WhatsApp, Telegram
  - **Custom**: Webhooks and Console (for testing)

- **Retry & Failure Handling**
  - Automatic retry mechanism for failed deliveries
  - Configurable retry limits
  - Comprehensive failure tracking and logging

- **Subscription Management**
  - Multiple subscription plans (Free, Pro, Enterprise)
  - Usage limits based on subscription tier
  - Extra-credit top-ups via Stripe payment integration
  - Subscription validation before notification delivery

- **Template Engine**
  - Dynamic notification templates
  - Support for placeholders and variable substitution
  - Template management per event type

- **Monitoring & Tracking**
  - Notification status tracking
  - Delivery statistics and usage analytics
  - System notifications for in-app subscriber messages
  - Subscriber data export

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

For detailed architecture documentation, see [docs/Architecture.md](docs/Architecture.md).

---

## Tech Stack

### Frontend
- **Framework**: React 19 with TypeScript 5
- **Build Tool**: Vite (Rolldown)
- **Routing**: React Router 7
- **Styling**: Tailwind CSS 3
- **Email Design**: GrapesJS + MJML (visual editor), Monaco Editor (code editor)
- **HTTP Client**: Axios (with automatic token refresh)

### Backend
- **Framework**: ASP.NET Core 10.0
- **Language**: C# 13
- **Architecture**: Clean Architecture / Onion Architecture
- **API**: RESTful API with JSON serialization
- **Validation**: FluentValidation
- **Messaging**: MediatR (CQRS pattern)

### Infrastructure
- **Database**: PostgreSQL 15 with Entity Framework Core 10.0
- **ORM**: Entity Framework Core with Migrations
- **Message Broker**: MassTransit with RabbitMQ
- **Caching**: Redis (distributed caching and workflow scheduling)
- **Payments**: Stripe integration for extra-credit top-ups
- **Containerization**: Docker & Docker Compose

### Notification Channels
- **Email**: SMTP / MailKit with MJML responsive templates
- **SMS**: SMS Gateway Integration
- **Push**: Firebase Cloud Messaging (FCM), Web Push (VAPID)
- **Collaboration**: Discord, Slack, MS Teams
- **Messaging**: WhatsApp, Telegram
- **Custom**: Webhooks, Console

### Development Tools
- **IDE**: Visual Studio / Rider / VS Code
- **Version Control**: Git & GitHub
- **Build System**: .NET CLI / MSBuild
- **Testing**: xUnit, FluentAssertions, Moq

### Planned Integrations
- **Advanced Scheduling**: Extended cron-based workflow scheduling

---

## Project Structure

```
Fertile-Notify/
├── backend/
│   ├── FertileNotify.API/              # API Layer - Controllers & Endpoints
│   │   ├── Authentication/             # JWT & API Key authentication handlers
│   │   ├── Authorization/              # API key scope policies
│   │   ├── Controllers/                # API Controllers (Auth, Notifications, Subscribers,
│   │   │                               #   Templates, Recipients, Statistics, Log,
│   │   │                               #   Payments, SystemNotifications)
│   │   ├── Extensions/                 # Service registration extension methods
│   │   ├── Middlewares/                # Exception handling, request logging
│   │   ├── Models/                     # Request/Response DTOs (by feature)
│   │   ├── Validators/                 # FluentValidation validators (by feature)
│   │   └── Program.cs                  # Application entry point
│   │
│   ├── FertileNotify.Application/      # Application Layer - Use Cases
│   │   ├── Contracts/                  # MassTransit message contracts
│   │   ├── DTOs/                       # Data Transfer Objects (by feature)
│   │   ├── Interfaces/                 # Service and repository contracts
│   │   ├── Services/                   # Application services (TemplateEngine, etc.)
│   │   └── UseCases/                   # Use case handlers (by feature)
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
│   │   ├── Authentication/             # JWT token & OTP services
│   │   ├── BackgroundJobs/             # MassTransit consumers & hosted workers
│   │   ├── Migrations/                 # EF Core migrations
│   │   ├── Notifications/              # Notification senders (Email, SMS, Push, etc.)
│   │   ├── Payment/                    # Stripe payment service
│   │   └── Persistence/                # EF Core DbContext & repositories
│   │
│   ├── FertileNotify.Tests/            # Unit & Integration Tests
│   │
│   ├── Dockerfile                      # Docker configuration
│   └── FertileNotify.sln               # Solution file
│
├── frontend/                           # React + TypeScript frontend application
│   ├── src/
│   │   ├── api/                        # API service layer
│   │   ├── components/                 # Reusable UI components
│   │   ├── constants/                  # Shared constants (channels, event types)
│   │   ├── pages/                      # Page components
│   │   └── types/                      # TypeScript type definitions
│   ├── Dockerfile                      # Docker configuration
│   └── README.md                       # Frontend documentation
│
├── docs/                               # Documentation
│   └── Architecture.md                 # Architecture documentation
│
├── docker-compose.yml                  # Docker Compose configuration
├── .env.example                        # Environment variables template
├── LICENSE                             # GPL-3.0 License
└── README.md                           # This file
```

---

## Getting Started

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) or higher
- [PostgreSQL 15](https://www.postgresql.org/download/) or higher (optional if using Docker)
- [Docker](https://www.docker.com/get-started) and [Docker Compose](https://docs.docker.com/compose/install/) (optional, recommended)
- [Redis](https://redis.io/) and [RabbitMQ](https://www.rabbitmq.com/) (optional if using Docker)
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

# Redis
REDIS_CONNECTION=localhost:6379

# RabbitMQ
RABBITMQ_HOST=localhost
RABBITMQ_USER=guest
RABBITMQ_PASS=guest

# Stripe (optional)
STRIPE_SECRET_KEY=your_stripe_secret_key
STRIPE_WEBHOOK_SECRET=your_stripe_webhook_secret
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

**Step 1** — `POST /api/auth/login` (sends OTP to your email)

**Step 2** — `POST /api/auth/verify-code` (returns JWT + refresh token)

Use the token in subsequent requests:
```
Authorization: Bearer {token}
```

### Register a Subscriber

**Endpoint**: `POST /api/subscribers/register`

**Request Body**:
```json
{
  "companyName": "Acme Corp",
  "email": "admin@acme.com",
  "password": "S3cur3P@ss!",
  "subscriptionPlan": "Pro"
}
```

### Send a Notification

**Endpoint**: `POST /api/notifications/send`

**Request Body**:
```json
{
  "channels": ["Email"],
  "recipients": ["user@example.com"],
  "subject": "Welcome!",
  "body": "Hello {{name}}, thanks for signing up!"
}
```

### Subscription Plans

| Feature | Free | Pro | Enterprise |
|---------|------|-----|------------|
| Monthly Notification Limit | 100 | 1,000 | 10,000 |
| Rate Limit (req/min) | 50 | 100 | 1,000 |
| Available Channels | Email | Email, SMS, Push | All |
| Allowed Event Types | Basic | Extended | All |

Extra credits can be purchased at any time via the Stripe-powered payment endpoint (`POST /api/payments/extra-credits/intent`).

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
- React-based subscriber frontend ✅ (Completed)
- RabbitMQ + MassTransit message broker integration ✅ (Completed)
- Redis caching and workflow scheduling ✅ (Completed)
- Multi-channel notification senders ✅ (Completed)
- Stripe payment integration for extra credits ✅ (Completed)
- Unit and integration tests ✅ (In Progress)
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
