namespace ClinicQueue.Api.DTOs;

    // Represents the data needed to update an existing appointment
public class UpdateAppointmentDto
{
    public string PatientName { get; set; } = null!;
    public string PatientContact { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string ClinicId { get; set; } = null!;

} 
