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
using SQLitePCL;
using Microsoft.EntityFrameworkCore.Query;

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
            _logger.LogInformation("FakeId has been retrieved successfully");

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task CreateAppointment_ReturnsCreatedAtActionResult_WhenAppointmentIsValid()
        {
            // Set up the test
            var controller = new AppointmentsController(_context, _cache, _logger);
            var newAppointment = new CreateAppointmentDto
            {
                PatientName = "James Milner",
                PatientContact = "123-456-7890",
                ScheduledAt = DateTime.UtcNow.AddHours(1), // Future date
                ClinicId = "ClinicH1"
            };

            // Perform the action
            var result = await controller.CreateAppointment(newAppointment);
            _logger.LogInformation("New appointment has been created successfully.");

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var createdAppointment = Assert.IsType<Appointment>(createdAtActionResult.Value);
            Assert.Equal("James Milner", createdAppointment.PatientName);
            Assert.Equal(1, _context.Appointments.Count());
        }

        [Fact]
        public async Task GetAppointmentById_ReturnsAppointment_WhenExists()
        {
            // Set up the test
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                PatientName = "Sarah Connor",
                PatientContact = "987-654-3210",
                ScheduledAt = DateTime.UtcNow.AddHours(2),
                ClinicId = "ClinicH2",
                Status = "Pending"
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            var controller = new AppointmentsController(_context, _cache, _logger);

            // Perform the action
            var result = await controller.GetAppointmentById(appointment.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedAppointment = Assert.IsType<Appointment>(okResult.Value);
            Assert.Equal("Bob Marley", returnedAppointment.PatientName);
        }
    }
}