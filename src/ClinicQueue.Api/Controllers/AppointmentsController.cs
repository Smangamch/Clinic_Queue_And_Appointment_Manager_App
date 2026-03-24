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

    [HttpPost]
    public async Task<IActionResult> Create(CreateAppointmentDto dto)
    {
        var result = await _appointmentService.CreateAsync(dto);
        return Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var appointments = await _appointmentService.GetAllAsync();
        return Ok(appointments);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var appointment = await _appointmentService.GetByIdAsync(id);
        if (appointment == null) return NotFound();
        return Ok(appointment);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _appointmentService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, UpdateAppointmentDto updatedAppointment)
    {
        var appointment = await _appointmentService.UpdateAsync(id, updatedAppointment);

        if (appointment == null) 
            return NotFound();
        
        return Ok(appointment);
    }

    [HttpGet("{id}/queue-position")]
    public async Task<IActionResult> GetQueuePosition(Guid id)
    {
        var queuePosition = await _appointmentService.GetQueuePositionAsync(id);
        if (queuePosition == null) 
            return NotFound();

        return Ok(queuePosition);
    }

    [HttpGet("paged")]
    public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 10, string? status = null, string? sortBy = null, string? sortOrder = "asc")
    {
        var pagedResult = await _appointmentService.GetPagedAsync(page, pageSize, status, sortBy, sortOrder);
        return Ok(pagedResult);
    }

    [HttpGet("today")]
    public async Task<IActionResult> GetTodayQueue()
    {
        var todayQueue = await _appointmentService.GetTodayQueueAsync();
        return Ok(todayQueue);
    }
}

