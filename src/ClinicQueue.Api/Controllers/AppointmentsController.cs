/// <summary>
/// Represents the API controller for managing appointments in the Clinic Queue system.
/// </summary>
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicQueue.Domain.Entities;
using ClinicQueue.Application.DTOs;
using Microsoft.Extensions.Caching.Memory; 
using ClinicQueue.Application.Services;

namespace ClinicQueue.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>
    /// Creates a new appointment.
    /// </summary>
    /// <param name="dto">Appointment creation payload.</param>
    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentDto dto)
    {
        var result = await _appointmentService.CreateAsync(dto);
        return Ok(result);
    }

    /// <summary>
    /// Retrieves all appointments.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await _appointmentService.GetAllAsync();
        return Ok(appointments);
    }

    /// <summary>
    /// Gets a single appointment by ID.
    /// </summary>
    /// <param name="id">Appointment ID.</param>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null) return NotFound();
        return Ok(appointment);
    }

    /// <summary>
    /// Deletes an appointment by ID.
    /// </summary>
    /// <param name="id">Appointment ID.</param>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _appointmentService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Updates appointment details.
    /// </summary>
    /// <param name="id">Appointment ID.</param>
    /// <param name="updatedAppointment">Updated appointment payload.</param>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateAppointmentDto updatedAppointment)
    {
        var appointment = await _appointmentService.UpdateAsync(id, updatedAppointment);

        if (appointment == null) 
            return NotFound();
        
        return Ok(appointment);
    }

    /// <summary>
    /// Gets queue position for a specified appointment.
    /// </summary>
    /// <param name="id">Appointment ID.</param>
    [HttpGet("{id}/queue-position")]
    public async Task<IActionResult> GetQueuePosition(Guid id)
    {
        var queuePosition = await _appointmentService.GetQueuePositionAsync(id);
        if (queuePosition == null) 
            return NotFound();

        return Ok(queuePosition);
    }

    /// <summary>
    /// Gets a paginated list of appointments with optional status and sort options.
    /// </summary>
    /// <param name="page">Page number (default 1).</param>
    /// <param name="pageSize">Page size (default 10).</param>
    /// <param name="status">Optional status filter.</param>
    /// <param name="sortBy">Optional sort field.</param>
    /// <param name="sortOrder">Optional sort order ('asc' or 'desc').</param>
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10, string? status = null, string? sortBy = null, string? sortOrder = "asc")
    {
        var pagedResult = await _appointmentService.GetPagedAsync(page, pageSize, status, sortBy, sortOrder);
        return Ok(pagedResult);
    }

    /// <summary>
    /// Returns today's queue of appointments.
    /// </summary>
    [HttpGet("today")]
    public async Task<IActionResult> GetTodayQueue()
    {
        var todayQueue = await _appointmentService.GetTodayQueueAsync();
        return Ok(todayQueue);
    }

    /// <summary>
    /// Queries appointments with advanced filtering/sorting/pagination.
    /// </summary>
    /// <param name="query">The query parameters model.</param>
    [HttpGet("query")]
    public async Task<IActionResult> QueryAppointments([FromQuery] AppointmentQueryDto query)
    {
        var result = await _appointmentService.QueryAppointmentsAsync(query);
        return Ok(result);
    }
}

