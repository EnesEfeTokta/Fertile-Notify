# Fertile Notify

Fertile Notify is an event-driven notification platform designed to centralize and manage notification delivery across multiple channels.

The system receives events from external applications, processes notification rules, and delivers messages asynchronously using background workers with retry mechanisms.

## 🚀 Core Features
- Event-based notification ingestion
- Asynchronous background processing
- Multi-channel notification delivery (Email, In-App)
- Retry & failure handling
- Subscription-based usage limits
- Admin dashboard for monitoring events and notifications

## 🧠 Architecture Overview
Fertile Notify follows a clean, layered architecture:

- API Layer – Event ingestion & authentication
- Application Layer – Business logic & decision making
- Domain Layer – Core entities and rules
- Infrastructure Layer – Persistence & background workers

## 🔄 Event Flow
1. External system sends an event
2. Event is validated and processed
3. Notification decisions are evaluated
4. Messages are queued for delivery
5. Background workers deliver notifications
6. Failures are retried or marked as failed

## 🛠️ Tech Stack
- Backend: ASP.NET Core
- Frontend: React
- Authentication: JWT
- Database: SQL Server
- Background Jobs: Hosted Services

## 📌 Project Status
This project is under active development.  
Initial focus is on core event processing and notification delivery.
