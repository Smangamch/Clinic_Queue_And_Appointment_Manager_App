# ClinicQueue App

## About the project
The Clinic Queue & Appointment Manager is a lightweight application designed to help community clinics and small healthcare practices reduce waiting times and improve patient flow management. The system enables patients to book, reschedule, or cancel appointments, while clinic staff can manage daily queues, check in patients, and monitor real-time updates.

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
