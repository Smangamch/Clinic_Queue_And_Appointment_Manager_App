public class AppointmentResponseDto
{
    public Guid Id { get; set; }
    public string PatientName { get; set; } = null!;
    public string PatientContact { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string ClinicId { get; set; } = null!;
    public bool CheckedIn {get; set;}
    public string Status {get; set;} = null!;
}