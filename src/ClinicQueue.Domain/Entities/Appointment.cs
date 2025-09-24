public class Appointment{
// We create properties for the required entities for an appointment
    public Guid Id { get; set; } = Guid.NewGuid();
    public string PatientName { get; set; } = Null!;
    public PatientContact { get; set; } null!;
    public DateTime ScheduledAt { get; set; };
    public bool CheckedIn { get; set; };
    public string ClinicId { get; set; } = null!;

}