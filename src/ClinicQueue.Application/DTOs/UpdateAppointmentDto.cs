namespace ClinicQueue.Application.DTOs;

    // Represents the data needed to update an existing appointment
public class UpdateAppointmentDto
{
    public string PatientName { get; set; } = null!;
    public string PatientContact { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string CheckedIn { get; set; } = null!; // Accepting as string to handle form data, will be converted to bool in service
    public string Status { get; set; } = null!;
} 

