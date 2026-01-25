# Fertile Notify

**Fertile Notify** is a robust, event-driven notification platform designed to centralize and manage notification delivery across multiple channels. Built with **.NET 9** and adhering to **Clean Architecture** principles, it provides a scalable foundation for handling asynchronous communication.

> **Note:** This project is currently in **Sprint 1 (Prototype Phase)**. Persistence is handled via In-Memory repositories for rapid development and testing.

---

## 🚀 Key Features (Sprint 1)

- **Event-Driven Architecture**: Decoupled event ingestion and processing.
- **Asynchronous Processing**: Background workers handle notification delivery to ensure API responsiveness.
- **Multi-Channel Support**: Architecture supports Email, SMS, and Console (Log) channels.
- **Clean Architecture**: Strict separation of concerns (API, Application, Domain, Infrastructure).
- **Security**: JWT (JSON Web Token) Authentication.

---

## 🛠️ Technology Stack

- **Framework**: ASP.NET Core (.NET 9.0)
- **Language**: C#
- **Architecture**: Clean Architecture
- **Data Storage**: In-Memory (Current State)
- **Authentication**: JWT Bearer
- **Validation**: FluentValidation
- **Documentation**: Swagger / OpenAPI

---

## 📂 Project Structure

The solution follows a modular structure:

- **`FertileNotify.API`**: The entry point. Handles HTTP requests, configuration, and dependency injection.
- **`FertileNotify.Application`**: Contains business logic, use cases, and interfaces.
- **`FertileNotify.Domain`**: Core entities, value objects, and domain logic.
- **`FertileNotify.Infrastructure`**: Implementation of interfaces (repositories, background jobs, external services).

---

## ⚙️ Getting Started

Follow these steps to set up and run the project locally.

### Prerequisites

- [**.NET 9.0 SDK**](https://dotnet.microsoft.com/download/dotnet/9.0) installed.

### Installation

1. **Clone the repository:**
   ```bash
   git clone <repository-url>
   cd FertileNotify
   ```

2. **Navigate to the backend directory:**
   ```bash
   cd backend
   ```

3. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

### Configuration

The application uses `appsettings.json` for configuration. The default configuration includes settings for JWT:

```json
"JwtSettings": {
  "SecretKey": "YourSuperSecretKeyHere",
  "Issuer": "FertileNotifyAPI",
  "Audience": "FertileNotifyClients",
  "ExpiryInMinutes": 60
}
```

> **Security Warning**: For production use, always replace `SecretKey` with a strong, secure value and manage it via User Secrets or Environment Variables.

### Running the Application

To start the API:

```bash
dotnet run --project FertileNotify.API
```

The API will start. You can access the **Swagger UI** to explore the endpoints at:
`http://localhost:<port>/swagger` (Port varies, typically 5000-5200)

---

## 🔌 Usage

### Authentication

The system currently uses an In-Memory user repository. You can obtain a JWT token using the `login` endpoint by providing a User ID.

**Request:** `POST /api/auth/login`

```json
{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
}
```

*Note: Ensure the User ID exists in the in-memory seed data or register a user if a registration endpoint is available.*

---

## 🗺️ Roadmap

- [x] Core Event Processing
- [x] In-Memory Persistence
- [x] Basic JWT Auth
- [ ] SQL Server Integration (Entity Framework Core)
- [ ] Real Email/SMS Service Integrations (SendGrid, Twilio, etc.)
- [ ] User Registration & Management UI
- [ ] Notification Templates Management

---

## 📄 License

This project is licensed under the MIT License.
