using ClinicQueue.Domain.Entities;

namespace ClinicQueue.Application.Services;
{
    
}

public interface IAppointmentService
{
    Task<Appointment> CreateAsync(Appointment appointment);
    Task<IEnumerable<Appointment>> GetAllAsync();
    Task<Appointment?> GetByIdAsync(Guid id);
    Task<bool> DeleteAsync(Guid id);
    Task<Appointment> UpdateAsync(Guid id, Appointment updatedAppointment);
}