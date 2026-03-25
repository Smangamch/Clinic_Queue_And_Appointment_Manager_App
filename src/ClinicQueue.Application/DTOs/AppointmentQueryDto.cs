namespace ClinicQueue.Application.DTOs;

public class AppointmentQueryDto
{
    public string? SortBy { get; set; } // status, scheduledAt, patientName
    public string? SortOrder { get; set; } // asc, desc, asc
    public string? Status {get; set;}
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}