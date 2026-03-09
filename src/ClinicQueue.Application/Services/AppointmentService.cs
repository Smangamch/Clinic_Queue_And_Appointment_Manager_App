using ClinicQueue.Domain.Entities;
using ClinicQueue.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace ClinicQueue.Application.Services

public class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _context;

    public AppointmentService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment> CreateAppointment(Appointment appointment)
    {
        // Validate that the scheduled time is in the future, preventing appointments from being set in the past.
        if(appointment.ScheduledAt <= DateTime.UtcNow)
        {
            throw new ArgumentException("Scheduled time must be in the future or cannot be in the past.");
        }

        // Add the new appointment to the database context and save changes to persist it in the database.
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }


    public async<Task<IEnumerable<Appointment>>> GetAllAsync()
    {
        // Retrieves all appointments without tracking changes for better performance.
        return await _context.Appointments
        .AsNoTracking()
        .ToListAsync();
    }

    public async Task<Appointment?> GetByIdAsync(int id)
    {
        // Retrieve appointment by id without tracking changes for better performance
        return await _context.Appointments
        .AsNoTracking()
        .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<bool> DeleteAppointment(Guid id)
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

    public async Task<Appointment> UpdateAsync(Guid id, Appointment updatedAppointment)
    {
        // Find the existing appointment by id, update its properties with the new values, and save changes to persist the updates.
        var appointment = await _context.Appointments.FindAsync(id);

        if(appointment == null)
        {
            return null;
        }

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
}