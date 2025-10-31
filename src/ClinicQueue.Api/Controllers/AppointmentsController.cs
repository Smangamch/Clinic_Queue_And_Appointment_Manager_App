/// <summary>
/// Represents the API controller for managing appointments in the Clinic Queue system.
/// </summary>
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicQueue.Domain.Entities;
using ClinicQueue.Api.DTOs;
using Microsoft.Extensions.Caching.Memory; 

namespace ClinicQueue.Api.Controllers // Fixed namespace declaration
{
    // This is the REST API controller which inherits from the ControllerBase
    [ApiController]
    [Route("api/[controller]")]

    public class AppointmentsController : ControllerBase
    {
        // Declares a readonly field to hold the database context for consistent CRUD operations on appointments.
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AppointmentsController> _logger; // Logger for logging information and errors
        private readonly IMemoryCache _cache; // Memory cache for caching appointment data

        // Constructor that accepts the database context via dependency injection and assigns it to the readonly field.
        public AppointmentsController(ApplicationDbContext context, IMemoryCache cache, ILogger<AppointmentsController> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        // Handles POST /api/appointments: validates future appointment time, creates and saves an appointment, returns 201 with resource URI.
        [HttpPost]
        public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto dto)
        {
            // This condition validates that the appointment time is in the future
            if (dto.ScheduledAt <= DateTime.UtcNow)
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
            _logger.LogInformation($"Fetching appointment with ID: {id}");

            if (appointment == null)
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

        // Updates an existing appointment, validates future time, returns 204 or 404 if not found.
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateAppointment(Guid id, [FromBody] UpdateAppointmentDto dto)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            _logger.LogInformation($"Updating appointment with ID: {id}");

            if (appointment == null)
                return NotFound();

            if (dto.ScheduledAt <= DateTime.UtcNow)
                return BadRequest("Appointment time must be in the future.");

            // Update the appointment details
            appointment.PatientName = dto.PatientName;
            appointment.PatientContact = dto.PatientContact;
            appointment.ScheduledAt = dto.ScheduledAt;
            appointment.ClinicId = dto.ClinicId;

            await _context.SaveChangesAsync();
            _logger.LogInformation($"Appointment with ID: {id} has been updated.");

            return NoContent(); // 204 No Content
        }

        // Deletes an appointment by ID, returning 204 if successful or 404 if not found.
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            _logger.LogInformation($"Deleting appointment with ID: {id}");

            if (appointment == null)
                return NotFound();

            _context.Appointments.Remove(appointment); // Remove the appointment from the database
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Appointment with ID: {id} has been deleted.");

            return NoContent();

        }

        // This endpoint filters appointments by clinic ID, returning 200 with list or 404 if none found.
        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByClinic(string clinicId)
        {
            var appointments = await _context.Appointments
                .Where(a => a.ClinicId == clinicId)
                .ToListAsync();

            if (appointments == null || !appointments.Any())
                return NotFound($"\nNo appointments found for clinic ID {clinicId}.");

            return Ok(appointments);
        }

        [HttpPut("{id:guid}/status")]
        // Updates the status of an appointment, returning 200 with confirmation or 404 if not found.
        public async Task<IActionResult> UpdateAppointmentStatus(Guid id, [FromBody] string status)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            _logger.LogInformation($"Updating status of appointment with ID: {id} to {status}");

            if (appointment == null)
                return NotFound();

            appointment.Status = status;
            await _context.SaveChangesAsync();
            _logger.LogInformation($"Appointment with ID: {id} status updated to {status}");

            return Ok($"\nAppointment status updated to {status}.");
        }

        [HttpGet]
        // This endpoint caches frequently used data for faster retrieval and returns those values
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
        {
            const string cacheKey = "appointments_cache";
            // This condition tries to retrieve from the cache first
            if (_cache.TryGetValue(cacheKey, out List<Appointment>? cachedAppointments))
            {
                Console.WriteLine("Returning appointments from cache...");
                return Ok(cachedAppointments);
            }

            // If not cached, then we fetch from the database
            var appointments = await _context.Appointments.ToListAsync();

            // Cache the results for 1 minute
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            _cache.Set(cacheKey, appointments, cacheOptions);

            Console.WriteLine("Caching appointments result...");
            return Ok(appointments);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Appointment>>> SearchAppointments(
            [FromQuery] string? query,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)

        {
            // Start with all appointments
            var appointments = _context.Appointments.AsQueryable();

            if (!string.IsNullOrEmpty(query))
            {
                // Filter by patient name or contact containing the query string
                appointments = appointments.Where(a =>
                    a.PatientName.Contains(query) ||
                    a.PatientContact.Contains(query));
            }

            // Apply pagination and ordering
            var pagedAppointments = await appointments
                .OrderBy(a => a.ScheduledAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(pagedAppointments);
        }
    }
}

