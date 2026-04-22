# Clinic Queue & Appointment Manager

A full-stack web application designed to modernize clinic appointment scheduling, queue management, and patient flow handling.

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

## Tech Stack

### Backend:
- .NET 8 Web API
- Entity Framework Core
- SQLite

### Frontend:
- React (Vite)
- TypeScript

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

## Author

Mangaliso Hlatswayo  
Full-Stack Software Developer | QA Engineer
