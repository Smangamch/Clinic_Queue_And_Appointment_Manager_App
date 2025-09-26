namespace ClinicQueue.Api.CTOs;

{
    public class CreateAppointmentDto{
        public string PatientName { get; set; } = null!;
        public string PatientContact { get; set; } = null!; // Assuming it's a string
        public DateTime ScheduledAt { get; set; }
        public string ClinicId { get; set; } = null!;
    }
}