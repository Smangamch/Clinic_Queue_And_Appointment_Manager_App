# ClinicQueue App

## Overview

This project is a backend RESTful API developed using ASP.NET Core (.NET 8) and Clean Architecture principles. The system is designed to modernize how public clinics manage patient appointments and queue flow by replacing manual booking methods with a structured and scalable digital solution.

The API allows clinics to create, update, and manage patient appointments while also providing real-time queue positioning, status tracking, and advanced data querying features.

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

### Key Features

- Create, update, delete, and manage clinic appointments
- Real-time queue number calculation based on scheduled time
- Status-based appointment filtering (Scheduled, Checked-In, Completed, Cancelled)
- Advanced querying with filtering, multi-field sorting, and pagination
- Global exception handling middleware
- Structured logging using ASP.NET Core built-in logging
- Clean Architecture structure (Domain, Application, Infrastructure, API layers)
- DTO-based request and response handling
- Fully testable API using Swagger

### Technologies Used

- ASP.NET Core (.NET 8)
- Entity Framework Core
- Clean Architecture
- LINQ (advanced filtering, sorting, and pagination)
- Swagger / OpenAPI
- C#

### Purpose of the Project

Many public clinics still rely on manual appointment booking methods, which often result in scheduling conflicts, long waiting times, and poor queue management. This system aims to solve that problem by providing a scalable backend service that can be integrated into web or mobile applications.

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
