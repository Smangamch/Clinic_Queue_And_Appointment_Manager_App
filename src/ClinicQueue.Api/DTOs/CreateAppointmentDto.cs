namespace ClinicQueue.Api.DTOs;

public class CreateAppointmentDto
{
    public string PatientName { get; set; } = null!;
    public string PatientContact { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string ClinicId { get; set; } = null!;
}