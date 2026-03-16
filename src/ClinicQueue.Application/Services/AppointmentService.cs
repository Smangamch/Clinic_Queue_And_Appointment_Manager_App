using ClinicQueue.Domain.Entities;
using ClinicQueue.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueue.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _context;

    public AppointmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        // Validate if any appointment exists at the same scheduled time to prevent double booking
        var exists = _context.Appointments.
            AnyAsync(a => a.ScheduledAt == appointment.ScheduledAt);

        // If an appointment already exists at the same time, throw an exception to indicate a scheduling conflict
        if(exists)
        {
            throw new InvalidOperationException("An appointment is already scheduled at this time.");
        }

        // Add the new appointment to the database context and save changes to persist it in the database
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }


    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        // Retrieves all appointments without tracking changes for better performance.
        return await _context.Appointments
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        // Retrieve appointment by id without tracking changes for better performance
        return await _context.Appointments
        .AsNoTracking()
        .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        // FInd the appointment by id and remove it from the database context, then save changes to persist the deletion.
        var appointment = await _context.Appointments.FindAsync(id);
        if(appointment == null)
        {
            return false;
        }

        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Appointment?> UpdateAsync(Guid id, Appointment updatedAppointment)
    {
        // Find the existing appointment by id, update its properties with the new values, and save changes to persist the updates.
        var appointment = await _context.Appointments.FindAsync(id);

        if(appointment == null)
        {
            return null;
        }
        
        // Validate that the new scheduled time is not in the past
        if(updatedAppointment.ScheduledAt <= DateTime.UtcNow)
        {
            throw new ArgumentException("Scheduled time must be in the future or cannot be in the past.");
        }

        appointment.PatientName = updatedAppointment.PatientName;
        appointment.ScheduledAt = updatedAppointment.ScheduledAt;
        appointment.PatientContact = updatedAppointment.PatientContact;

        await _context.SaveChangesAsync();

        return appointment;
    }

    public async Task<Appointment?> UpdateStatusAsync(Guid id, string newStatus)
    {
        var appointment = await _context.Appointments.FindAsync(id);

        if(appointment == null)
        {
            return null;
        }
        
        // Validate status transition rules (e.g., Pending -> Scheduled -> CheckedIn -> Completed)
        bool isValidTransition = 
            (appointment.Status == "Pending" && newStatus == "Scheduled") ||
            (appointment.Status == "Scheduled" && newStatus == "CheckedIn") ||
            (appointment.Status == "CheckedIn" && newStatus == "Completed");

        // If the transition is not valid, throw an exception or return an error
        if(!isValidTransition)
        {
            throw new InvalidOperationException($"Invalid status transition from {appointment.Status} to {newStatus}.");
        }

        appointment.Status = newStatus;
        await _context.SaveChangesAsync();
        return appointment;

    }

    public async Task<AppointmentQueueDto> GetQueuePositionAsync(Guid id)
    {
        /// <summary>
        /// Retrieves the appointment with the specified ID and its queue number based on scheduled time ordering.
        /// </summary>
        /// <remarks>
        /// The queue number is determined by the appointment's position in the ordered list.
        /// </remarks>
        var appointment = await _context.Appointments
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Appointment.Id == id);

        if(appointment == null)
        {
            throw new KeyNotFoundException("Appointment not found.");
        }

        return new AppointmentQueueDto
        {
            Id = appointment.Appointment.Id,
            QueueNumber = appointment.QueueNumber
        };
    }
}