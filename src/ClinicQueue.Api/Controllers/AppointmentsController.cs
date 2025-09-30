/// <summary>
/// Represents the API controller for managing appointments in the Clinic Queue system.
/// </summary>
/// <remarks>
/// This controller provides endpoints for creating, retrieving, updating, and deleting 
/// appointment records. It interacts with the application's database context to perform 
/// CRUD operations on appointment entities.
/// </remarks>
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicQueue.Domain.Entities;
using ClinicQueue.Api.DTOs;

namespace ClinicQueue.Api.Controllers // Fixed namespace declaration
{
    // This is the REST API controller which inherits from the ControllerBase
    [ApiController]
    [Route("api/[controller]")]

    public class AppointmentsController : ControllerBase
    {
        // Declares a readonly field to hold the database context for consistent CRUD operations on appointments.
        private readonly ApplicationDbContext _context;

        // Constructor that accepts the database context via dependency injection and assigns it to the readonly field.
        public AppointmentsController(ApplicationDbContext context)
        {
            _context = context;
        }
        // Handles POST /api/appointments: validates future appointment time, creates and saves an appointment, returns 201 with resource URI.
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            // This condition validates that the appointment time is in the future
            if(dto.ScheduledAt <= DateTime.UtcNow)
            {
                return BadRequest("Appointment time must be in the future.");
            }
        
            // Create a new appointment entity from the DTO
            var appointment = new Appointment
            {
                PatientName = dto.PatientName,
                PatientContact = dto.PatientContact,
                ScheduledAt = dto.ScheduledAt,
                ClinicId = dto.ClinicId
            };

            // Save the patient's information to the database
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetAppointmentById), // Fixed method name
                new { id = appointment.Id }, appointment);

        }

        // Retrieves an appointment by its ID, returning 200 with data or 404 if not found.
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Appointment>> GetAppointmentById(Guid id) // Added generic type
        {
            //Lookup the appointment by its ID in the database
            var appointment = await _context.Appointments.FindAsync(id);

            if(appointment == null)
            {
                return NotFound();
            }

            return Ok(appointment); 
        }

        // Handles GET /api/appointments: retrieves all appointments, returns 200 with list.
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAllAppointments() // Added generic type
        {
            var appointments = await _context.Appointments.ToListAsync();
            return Ok(appointments); 
        }
    }
}

