# ClinicQueue App

## Overview

ClinicQueue is a backend API for managing clinic appointment scheduling, queue position calculation, and appointment status workflow. It is built with modern C#/.NET architecture principles and designed for maintainability, testability, and real-world clinical operations.

## Table of Contents

- [Features](#features)
- [Architecture](#architecture)
- [API Endpoints](#api-endpoints)
- [Getting Started](#getting-started)
- [Testing](#testing)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [Author](#author)

## Features

### Appointment Management

- Create, read, update, and delete appointments
- Prevents scheduling conflicts (double-booking)
- Business validation for required fields and date/time rules

### Queue Management

- Computes dynamic daily queue position by appointment time
- Returns daily queue list with position and status
- Supports queue lookup by appointment ID

### Status Workflow

- Enforces controlled transitions:
  - `Pending` → `Scheduled` → `CheckedIn` → `Completed`
- Protects against invalid status updates

### Pagination and Filtering

- Supports paged responses with `page` and `pageSize`
- Supports filtering by status and date range
- Combines filtering and pagination in one endpoint

### API Documentation

- Integrated Swagger UI for interactive API exploration
- Automatically generated OpenAPI documentation

## Architecture

- ASP.NET Core Web API
- Clean architecture separation (API, Application, Domain, Infrastructure)
- Entity Framework Core (EF Core)
- DTO-based API contract
- Async/await for scalable I/O operations

## API Endpoints

### Appointments

- `GET /api/appointments`
- `GET /api/appointments/{id}`
- `POST /api/appointments`
- `PUT /api/appointments/{id}`
- `DELETE /api/appointments/{id}`

### Advanced & Utility

- `GET /api/appointments/paged` — pagination + filtering
- `GET /api/appointments/today` — today's queue list
- `GET /api/appointments/{id}/queue` — queue position for one appointment

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQLite (for local dev database)
- Git

### Local Setup

1. Clone repository

```bash
git clone https://github.com/<your-username>/ClinicQueue.git
cd ClinicQueue
```

2. Restore packages

```bash
dotnet restore
```

3. Apply migrations

```bash
dotnet ef database update --project src/ClinicQueue.Api
```

4. Run API

```bash
dotnet run --project src/ClinicQueue.Api
```

5. Open API docs/Swagger

```text
https://localhost:5188/swagger
```

## Testing

### Unit Tests

Run unit tests:

```bash
dotnet test ClinicQueue.Tests/ClinicQueue.Tests.csproj
```

### Integration Tests

Run integration tests:

```bash
dotnet test ClinicQueue.IntegrationTests/ClinicQueue.IntegrationTests.csproj
```

## Roadmap

- Add patient search by name and contact
- Implement authentication/authorization (JWT)
- Add ILogger structured logging
- Add FluentValidation for request input
- Add Docker support and containerized deployment

## Contributing

1. Fork the repo
2. Create feature branch
3. Add tests for new logic
4. Open PR with clear description

## Author

Mangaliso Hlatswayo

Software Quality Assurance Engineer | Backend Developer (C#/.NET)
