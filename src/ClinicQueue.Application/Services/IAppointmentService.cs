using ClinicQueue.Domain.Entities;
using ClinicQueue.Application.DTOs;

namespace ClinicQueue.Application.Services
{
    public interface IAppointmentService
    {
        Task<Appointment> CreateAsync(Appointment appointment);
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<Appointment?> UpdateAsync(Guid id, Appointment updatedAppointment);
        Task<AppointmentQueueDto> GetQueuePositionAsync(Guid id);
        Task<PagedResult<Appointment>> GetPagedAsync(int page, int pageSize, string? status);
        Task<IEnumerable<TodayQueueDto>> GetTodayQueueAsync();
    }
}