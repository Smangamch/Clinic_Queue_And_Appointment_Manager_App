namespace ClinicQueue.Application.DTOs;

public class TodayQueueDto
{
    public int QueueNumber { get; set; }
    public string PatientName { get; set; } = null!;
    public DateTime ScheduledAt { get; set; }
    public string Status { get; set; } = null!;
}