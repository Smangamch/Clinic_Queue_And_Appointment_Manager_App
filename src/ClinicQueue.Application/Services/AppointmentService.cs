using ClinicQueue.Domain.Entities;
using ClinicQueue.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ClinicQueue.Application.DTOs;


namespace ClinicQueue.Application.Services;

/// <summary>
/// Appointment service implementing appointment operations and business rules.
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _context;

    public AppointmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new appointment if no scheduling conflict exists.
    /// </summary>
    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        var exists = await _context.Appointments.AnyAsync(a => a.ScheduledAt == appointment.ScheduledAt);
        if (exists)
        {
            throw new InvalidOperationException("An appointment is already scheduled at this time.");
        }

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }


    /// <summary>
    /// Returns all appointments (read-only query for performance).
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        return await _context.Appointments.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Finds a single appointment by id or returns null when missing.
    /// </summary>
    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        return await _context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <summary>
    /// Deletes appointment by id; returns false when the record does not exist.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if(appointment == null)
        {
            return false;
        }

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Updates appointment details and returns updated entity.
    /// </summary>
    public async Task<Appointment?> UpdateAsync(Guid id, Appointment updatedAppointment)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return null;
        }

        if (updatedAppointment.ScheduledAt <= DateTime.UtcNow)
        {
            throw new ArgumentException("Scheduled time must be in the future or cannot be in the past.");
        }

        appointment.PatientName = updatedAppointment.PatientName;
        appointment.ScheduledAt = updatedAppointment.ScheduledAt;
        appointment.PatientContact = updatedAppointment.PatientContact;

        await _context.SaveChangesAsync();
        return appointment;
    }

    /// <summary>
    /// Updates appointment status using defined state transitions and enforces business rules.
    /// </summary>
    public async Task<Appointment?> UpdateStatusAsync(Guid id, string newStatus)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return null;
        }

        bool isValidTransition =
            (appointment.Status == "Pending" && newStatus == "Scheduled") ||
            (appointment.Status == "Scheduled" && newStatus == "CheckedIn") ||
            (appointment.Status == "CheckedIn" && newStatus == "Completed");

        if (!isValidTransition)
        {
            throw new InvalidOperationException($"Invalid status transition from {appointment.Status} to {newStatus}.");
        }

        appointment.Status = newStatus;
        await _context.SaveChangesAsync();
        return appointment;
    }

    /// <summary>
    /// Computes queue position for an appointment by counting earlier scheduled appointments.
    /// </summary>
    public async Task<AppointmentQueueDto> GetQueuePositionAsync(Guid id)
    {
        var appointment = await _context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        if (appointment == null)
        {
            throw new KeyNotFoundException("Appointment not found.");
        }

        // Calculate the queue number by counting how many appointments are scheduled before the current appointment's scheduled time.
        var queueNumber = await _context.Appointments
            .CountAsync(a => a.ScheduledAt < appointment.ScheduledAt) + 1; // +1 to convert from 0-based index to 1-based queue number

        return new AppointmentQueueDto
        {
            AppointmentId = appointment.Id,
            QueueNumber = queueNumber
        };
    }

    /// <summary>
    /// Returns paged appointment results with optional status filtering.
    /// </summary>
    public async Task<PagedResult<Appointment>> GetPagedAsync(int page, int pageSize, string? status)
    {
        var query = _context.Appointments.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(a => a.Status == status);
        }

        var totalRecords = await query.CountAsync();
        var appointments = await query
            .OrderBy(a => a.ScheduledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Appointment>
        {
            Data = appointments,
            TotalRecords = totalRecords,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Returns appointments scheduled for today with computed queue numbers.
    /// </summary>
    public async Task<IEnumerable<TodayQueueDto>> GetTodayQueueAsync()
    {
        var today = DateTime.UtcNow.Date;

        var todayAppointments = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.ScheduledAt.Date == today)
            .OrderBy(a => a.ScheduledAt)
            .ToListAsync();

        return todayAppointments.Select((a, index) => new TodayQueueDto
        {
            QueueNumber = index + 1,
            PatientName = a.PatientName,
            ScheduledAt = a.ScheduledAt,
            Status = a.Status
        });
    }

}