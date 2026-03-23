using ClinicQueue.Domain.Entities;
using ClinicQueue.Application.DTOs;

namespace ClinicQueue.Application.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto);
        Task<IEnumerable<Appointment>> GetAllAsync();
        Task<Appointment?> GetByIdAsync(Guid id);
        Task<bool> DeleteAsync(Guid id);
        Task<AppointmentResponseDto?> UpdateAsync(Guid id, UpdateAppointmentDto updatedAppointment);
        Task<AppointmentQueueDto> GetQueuePositionAsync(Guid id);
        Task<PagedResult<Appointment>> GetPagedAsync(int page, int pageSize, string? status, string? sortBy, string? sortOrder);
        Task<IEnumerable<TodayQueueDto>> GetTodayQueueAsync();

    }
}