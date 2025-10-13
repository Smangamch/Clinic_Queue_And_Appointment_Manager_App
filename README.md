# ClinicQueue App

## About the project
The Clinic Queue & Appointment Manager App is a lightweight application designed to help community clinics and small healthcare practices reduce waiting times and improve patient flow management. The system enables patients to book, reschedule, or cancel appointments, while clinic staff can manage daily queues, check in patients, and monitor real-time updates.

From a development perspective, this project serves as a hands-on exploration of modern C# and .NET technologies, covering clean architecture, Entity Framework Core for persistence, ASP.NET Core Web API for service endpoints, and SignalR for real-time communication. The project is built incrementally with focus on separation of concern, enforcing clarity on testability, scalability, and maintainability — making it both a practical solution to a real-world problem and a valuable exercise in full-stack .NET application development. 

## Features
### Core features
* Create Appointments - Add new patient appointments with details like patient name, contact, clinic ID, and scheduled
* Retrieve Appointments - Fetch all appointments or a specific appointment by its unique ID.
* Update Appointments - Modify existing appointment details such as time, patient information, or clinic assignment.
* Delete Appointments - Permanently remove or cancel an appointment.

### Filtering and Querying
* Filter by Clinic - Retrieve all appointments for a specific clinic using its clinic ID.
* Filter by Date - Get all appointments scheduled for a particular date.
* Search (Upcoming Feature) - Search appointments by patient name or contact details **(to be added)**.

### Appointment Status Management
* Track Appointment Status - Each appointment includes a Status field __(Pending, Completed, or Cancelled)__.
* Update Status - Change appointment status using a dedicated API endpoint.

### Perfomance Optimization
* In-Memory Caching - Reduces database queries for frequently accessed appointment data.
* Lightweight Storage - Uses SQLite for a simple, file-based, and portable database but I have also added **MySQL** package to the project for later migration

## How to run the project
### Prerequisites
The following need to be installed in your machine:
* .NET 8 SDK: https://dotnet.microsoft.com/en-us/download
* SQLite: https://www.sqlite.org/download.html
* POSTMAN (Optional) - https://www.postman.com/downloads/
* Gitbash, but you can also use terminal provided in **Visual Studio Code**

### Setup instructions
1. Clone the repository
   * git clone https://github.com/<your-username>/ClinicQueue.git
   * cd ClinicQueue
2. Restore dependencies
   * dotnet restore
3. Apply migrations
   * dotnet ef database migrate
4. Run the application
   * dotnet run --project src/ClinicQueue.Api
5. Access the API
   * https://localhost:5188/swagger

