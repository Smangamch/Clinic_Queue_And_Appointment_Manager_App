public class Appointment{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PatientName { get; set; } = Null!;
    public PatientContact { get; set; } null!;
    public DateTime ScheduledAt { get; set; };
    public bool CheckedIn { get; set; };
    public string ClinicId { get; set; } = null!;

}