# Clinic Queue & Appointment Manager

A full-stack web application designed to modernize clinic appointment scheduling, queue management, and patient flow handling.

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Getting Started](#getting-started)
- [Author](#author)

##  Features

### Backend (ASP.NET Core)
- Appointment creation with validation (no double booking)
- Status lifecycle management: Pending → Scheduled → CheckedIn → Completed
- Queue position calculation
- Pagination support
- Advanced querying:
  - Filtering (status)
  - Sorting (date)
  - Pagination (page & size)
- Global exception handling
- Structured logging

### Frontend (React + Vite + TypeScript)
- Fetch and display appointments
- Create new appointments via form
- Real-time UI updates after creation
- API integration with backend

### Purpose of the Project

Many public clinics still rely on manual appointment booking methods, which often result in scheduling conflicts, long waiting times, and poor queue management. This system aims to solve that problem by providing a scalable backend service that can be integrated into web or mobile applications.

## Tech Stack

### Backend:
- .NET 8 Web API
- Entity Framework Core
- SQLite
- Clean Architecture
- LINQ (advanced filtering, sorting, and pagination)

### Frontend:
- React (Vite)
- TypeScript

## Architecture

- ASP.NET Core Web API
- Clean architecture separation (API, Application, Domain, Infrastructure)
- Entity Framework Core (EF Core)
- DTO-based API contract
- Async/await for scalable I/O operations

## API Endpoints

### Get Paginated Appointments
`GET /api/appointments/paged?page=1&pageSize=5`

### Create Appointment
`POST /api/appointments`

### Advanced Query
`GET /api/appointments/query?SortBy=scheduledAt&SortOrder=asc&Status=Scheduled&page=1&pageSize=5`

## ⚙️ Setup Instructions

### Backend
```bash
cd src/ClinicQueue.Api
dotnet run
```

Runs on: http://localhost:5188

### Frontend
```bash
cd frontend
npm install
npm run dev
```

Runs on: http://localhost:5173

## Testing

Swagger UI available at: http://localhost:5188/swagger

Tested scenarios:
- Valid appointment creation
- Duplicate time validation
- Past date rejection
- Pagination correctness

## System Design Considerations
- Prevents double booking
- Handles multiple concurrent users
- Designed for scalability across multiple clinics
- Clean separation of concerns (API, Application, Domain)

## Future Improvements
- Frontend pagination controls
- Filtering UI
- Real-time queue updates (WebSockets)
- Authentication & authorization
- Multi-clinic dashboard

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

## Author

Mangaliso Hlatswayo  
Full-Stack Software Developer | QA Engineer
