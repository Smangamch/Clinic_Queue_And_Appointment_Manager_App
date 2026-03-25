using ClinicQueue.Domain.Entities;
using ClinicQueue.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ClinicQueue.Application.DTOs;
using Microsoft.Extensions.Logging;


namespace ClinicQueue.Application.Services;

/// <summary>
/// Appointment service implementing appointment operations and business rules.
/// </summary>
public class AppointmentService : IAppointmentService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(ApplicationDbContext context, ILogger<AppointmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Creates a new appointment if no scheduling conflict exists.
    /// </summary>
    public async Task<AppointmentResponseDto> CreateAsync(CreateAppointmentDto dto)
    {
        var exists = await _context.Appointments.AnyAsync(a => a.ScheduledAt == dto.ScheduledAt);
        if (exists)
        {
            throw new InvalidOperationException("An appointment is already scheduled at this time.");
        }

        var appointment = new Appointment
        {
            Id = Guid.NewGuid(),
            PatientName = dto.PatientName,
            PatientContact = dto.PatientContact,
            ScheduledAt = dto.ScheduledAt,
            ClinicId = dto.ClinicId,
            CheckedIn = dto.CheckedIn,
            Status = dto.Status
        };

        _context.Appointments.Add(appointment);
        _logger.LogInformation("Appointment created successfully.");
        await _context.SaveChangesAsync();

        return new AppointmentResponseDto
        {
            Id = appointment.Id,
            PatientName = appointment.PatientName,
            PatientContact = appointment.PatientContact,
            ScheduledAt = appointment.ScheduledAt,
            ClinicId = appointment.ClinicId,
            CheckedIn = appointment.CheckedIn,
            Status = appointment.Status
        }; 
    }


    /// <summary>
    /// Returns all appointments (read-only query for performance).
    /// </summary>
    public async Task<IEnumerable<Appointment>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all appointments.");
        return await _context.Appointments.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Finds a single appointment by id or returns null when missing.
    /// </summary>
    public async Task<Appointment?> GetByIdAsync(Guid id)
    {
        _logger.LogInformation("Appointment retrieved successfully.");
        return await _context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <summary>
    /// Deletes appointment by id; returns false when the record does not exist.
    /// </summary>
    public async Task<bool> DeleteAsync(Guid id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if(appointment == null)
        {
            return false;
        }

        _context.Appointments.Remove(appointment);
        _logger.LogInformation("Appointment deleted successfully.");
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Updates appointment details and returns updated entity.
    /// </summary>
    public async Task<AppointmentResponseDto?> UpdateAsync(Guid id, UpdateAppointmentDto updatedAppointment)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return null;
        }

        if (updatedAppointment.ScheduledAt <= DateTime.UtcNow)
        {
            throw new ArgumentException("Scheduled time must be in the future or cannot be in the past.");
        }

        appointment.PatientName = updatedAppointment.PatientName;
        appointment.ScheduledAt = updatedAppointment.ScheduledAt;
        appointment.PatientContact = updatedAppointment.PatientContact;
        appointment.CheckedIn = bool.Parse(updatedAppointment.CheckedIn);
        appointment.Status = updatedAppointment.Status;

        _logger.LogInformation("Appointment updated successfully.");
        await _context.SaveChangesAsync();
        return new AppointmentResponseDto
        {
            PatientName = appointment.PatientName,
            PatientContact = appointment.PatientContact,
            ScheduledAt = appointment.ScheduledAt,
            CheckedIn = appointment.CheckedIn,
            Status = appointment.Status
        };
    }

    /// <summary>
    /// Updates appointment status using defined state transitions and enforces business rules.
    /// </summary>
    public async Task<Appointment?> UpdateStatusAsync(Guid id, string newStatus)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null)
        {
            return null;
        }

        bool isValidTransition =
            (appointment.Status == "Pending" && newStatus == "Scheduled") ||
            (appointment.Status == "Scheduled" && newStatus == "CheckedIn") ||
            (appointment.Status == "CheckedIn" && newStatus == "Completed");

        if (!isValidTransition)
        {
            throw new InvalidOperationException($"Invalid status transition from {appointment.Status} to {newStatus}.");
        }

        appointment.Status = newStatus;
        _logger.LogInformation("Appointment status updated successfully.");
        await _context.SaveChangesAsync();
        return appointment;
    }

    /// <summary>
    /// Computes queue position for an appointment by counting earlier scheduled appointments.
    /// </summary>
    public async Task<AppointmentQueueDto> GetQueuePositionAsync(Guid id)
    {
        var appointment = await _context.Appointments.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
        if (appointment == null)
        {
            _logger.LogWarning("Attempted to get queue position for non-existent appointment.");
            throw new KeyNotFoundException("Appointment not found.");
        }

        // Calculate the queue number by counting how many appointments are scheduled before the current appointment's scheduled time.
        var queueNumber = await _context.Appointments
            .CountAsync(a => a.ScheduledAt < appointment.ScheduledAt) + 1; // +1 to convert from 0-based index to 1-based queue number

        _logger.LogInformation("Queue position retrieved successfully.");
        return new AppointmentQueueDto
        {
            AppointmentId = appointment.Id,
            QueueNumber = queueNumber
        };
        
    }

    /// <summary>
    /// Returns paged appointment results with optional status filtering.
    /// </summary>
    public async Task<PagedResult<Appointment>> GetPagedAsync(int page, int pageSize, string? status, string? sortBy, string? sortOrder)
    {
        var query = _context.Appointments.AsQueryable();

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(a => a.Status == status);
        }

        if(!string.IsNullOrEmpty(sortBy))
        {
            if(sortBy == "ScheduledAt")
            {
                query = sortOrder == "desc" 
                ? query.OrderByDescending(a => a.ScheduledAt) 
                : query.OrderBy(a => a.ScheduledAt);
            }
            else if (sortBy == "Status")
            {
                query = sortOrder == "desc" 
                ? query.OrderByDescending(a => a.Status) 
                : query.OrderBy(a => a.Status);
            }
            else
            {
                query = query.OrderBy(a => a.ScheduledAt); // Default sorting
            }
        }
        else{
            query = query.OrderBy(a => a.ScheduledAt); // Default sorting
        }
        
        //Total count before pagination
        var totalRecords = await query.CountAsync();

        // Apply pagination
        var appointments = await query
            .OrderBy(a => a.ScheduledAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        _logger.LogInformation("Paged appointments retrieved successfully.");
        return new PagedResult<Appointment>
        {
            Data = appointments,
            TotalRecords = totalRecords,
            Page = page,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Returns appointments scheduled for today with computed queue numbers.
    /// </summary>
    public async Task<IEnumerable<TodayQueueDto>> GetTodayQueueAsync()
    {
        var today = DateTime.UtcNow.Date;

        var todayAppointments = await _context.Appointments
            .AsNoTracking()
            .Where(a => a.ScheduledAt.Date == today)
            .OrderBy(a => a.ScheduledAt)
            .ToListAsync();

        _logger.LogInformation("Today's queue retrieved successfully.");
        return todayAppointments.Select((a, index) => new TodayQueueDto
        {
            QueueNumber = index + 1,
            PatientName = a.PatientName,
            ScheduledAt = a.ScheduledAt,
            Status = a.Status
        });
    }

    /// <summary>
    /// Performs complex querying of appointments with filtering, sorting, and pagination based on the provided query parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering, sorting, and pagination.</param>
    /// <returns>A paged result containing the queried appointments.</returns>
    public async Task<PagedResult<AppointmentResponseDto>> QueryAppointmentsAsync(AppointmentQueryDto query)
    {
        // Start with all appointments as a queryable to allow for filtering, sorting, and pagination
        var appointments = _context.Appointments.AsQueryable();

        // Filtering
        if (!string.IsNullOrEmpty(query.Status))
        {
            appointments = appointments.Where(a => a.Status == query.Status);
        }

        // Sorting
        if (!string.IsNullOrWhiteSpace(query.SortBy))
        {
            // Support multiple sort fields separated by commas, e.g., "status,scheduledAt"
            var sortFields = query.SortBy.Split(',');
            var sortOrders = query.SortOrder?.Split(',') ?? new string[sortFields.Length];

            // Dynamically build the ordered query based on the specified sort fields and orders
            IOrderedQueryable<Appointment>? orderedQuery = null;

            // Use a loop to apply sorting for each specified field
            for (int i = 0; i < sortFields.Length; i++)
            {
                var field = sortFields[i].Trim().ToLower();
                var order = sortOrders.Length > i ? sortOrders[i].Trim().ToLower() : "asc";

                if (orderedQuery == null)
                {
                    orderedQuery = field switch
                    {
                        "status" => order == "desc"
                            ? appointments.OrderByDescending(a => a.Status)
                            : appointments.OrderBy(a => a.Status),

                        "scheduledat" => order == "desc"
                            ? appointments.OrderByDescending(a => a.ScheduledAt)
                            : appointments.OrderBy(a => a.ScheduledAt),

                        "patientname" => order == "desc"
                            ? appointments.OrderByDescending(a => a.PatientName)
                            : appointments.OrderBy(a => a.PatientName),

                        _ => appointments.OrderBy(a => a.ScheduledAt)
                    };
                }
                else
                {
                    orderedQuery = field switch
                    {
                        "status" => order == "desc"
                            ? orderedQuery.ThenByDescending(a => a.Status)
                            : orderedQuery.ThenBy(a => a.Status),

                        "scheduledat" => order == "desc"
                            ? orderedQuery.ThenByDescending(a => a.ScheduledAt)
                            : orderedQuery.ThenBy(a => a.ScheduledAt),

                        "patientname" => order == "desc"
                            ? orderedQuery.ThenByDescending(a => a.PatientName)
                            : orderedQuery.ThenBy(a => a.PatientName),

                        _ => orderedQuery
                    };
                }
            }
            // If no valid sort fields were provided, default to sorting by ScheduledAt
            appointments = orderedQuery ?? appointments;
        }

        var totalRecords = await appointments.CountAsync();

        _logger.LogInformation("Appointments queried successfully with filters and sorting.");
        var data = await appointments
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(a => new AppointmentResponseDto
            {
                Id = a.Id,
                PatientName = a.PatientName,
                PatientContact = a.PatientContact,
                ScheduledAt = a.ScheduledAt,
                CheckedIn = a.CheckedIn,
                ClinicId = a.ClinicId,
                Status = a.Status
            })
            .ToListAsync();

        _logger.LogInformation("Paged appointment query results retrieved successfully.");
        return new PagedResult<AppointmentResponseDto>
        {
            Page = query.Page,
            PageSize = query.PageSize,
            TotalRecords = totalRecords,
            Data = data
        };
    }
}