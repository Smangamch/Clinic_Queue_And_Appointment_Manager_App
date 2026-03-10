/// <summary>
/// Represents the API controller for managing appointments in the Clinic Queue system.
/// </summary>
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClinicQueue.Domain.Entities;
using ClinicQueue.Api.DTOs;
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
    public async Task<IActionResult> Create(Appointment appointment)
    {
        var result = await _appointmentService.CreateAsync(appointment);
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
    public async Task<IActionResult> Update(Guid id, Appointment updatedAppointment)
    {
        var appointment = await _appointmentService.UpdateAsync(id, updatedAppointment);

        if (appointment == null) 
            return NotFound();
        
        return Ok(appointment);
    }
}

