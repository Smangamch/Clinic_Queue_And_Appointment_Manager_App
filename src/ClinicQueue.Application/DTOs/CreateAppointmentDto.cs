namespace ClinicQueue.Application.DTOs;
            
public class CreateAppointmentDto
{
    public string PatientName { get; set; } = null!;
    public string PatientContact { get; set; }
    public DateTime ScheduledAt { get; set; }
    public string ClinicId { get; set; } = null!;
    public bool CheckedIn {get; set;}
    public string Status {get; set;} = null!;
    
}