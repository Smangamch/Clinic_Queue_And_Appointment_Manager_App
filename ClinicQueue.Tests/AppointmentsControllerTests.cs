/// <summary>
/// Contains unit tests for the AppointmentsController in the Clinic Queue system.
/// This class uses an in-memory database, memory cache, and logger to test the controller's
/// endpoints in isolation, ensuring correct behavior for appointment-related operations.
/// </summary>
using Xunit;
using ClinicQueue.Api.Controllers;
using ClinicQueue.Infrastructure;
using ClinicQueue.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using ClinicQueue.Api.DTOs;

namespace ClinicQueue.Tests
{
    public class AppointmentsControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;
        private readonly ILogger<AppointmentsController> _logger;

        public AppointmentsControllerTests()
        {
            // Set up in-memory database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("ClinicQueueTestDb")
                .Options;

            // Initialize the context, cache, and logger
            _context = new ApplicationDbContext(options);
            _cache = new MemoryCache(new MemoryCacheOptions());
            _logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<AppointmentsController>();
        }

        [Fact]
        public async Task GetAppointmentById_ReturnsNotFound_WhenAppointmentDoesNotExist()
        {
            // Set up the test
            var controller = new AppointmentsController(_context, _cache, _logger);
            var fakeId = Guid.NewGuid();

            // Perform the action
            var result = await controller.GetAppointmentById(fakeId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateAppointment_ReturnsCreatedAtActionResult_WhenAppointmentIsValid()
        {
            // Set up the test
            var controller = new AppointmentsController(_context, _cache, _logger);
            var dto = new CreateAppointmentDto
            {
                PatientName = "John Doe",
                PatientContact = "123-456-7890",
                ScheduledAt = DateTime.UtcNow.AddHours(1), // Future date
                ClinicId = Guid.NewGuid()
            };

            // Perform the action
            var result = await controller.CreateAppointment(dto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var appointment = Assert.IsType<Appointment>(createdAtActionResult.Value);
            Assert.Equal(dto.PatientName, appointment.PatientName);
            Assert.Equal(dto.PatientContact, appointment.PatientContact);
            Assert.Equal(dto.ScheduledAt, appointment.ScheduledAt);
            Assert.Equal(dto.ClinicId, appointment.ClinicId);
        }
    }
}