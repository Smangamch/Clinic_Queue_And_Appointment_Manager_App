using system.Net;
using System.Net.Http.Json;
using Xunit;
using ClinicQueue.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClinicQueue.IntegrationTests
{
    public class AppointmentsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        // Create an HttpClient to send requests to the test server
        private readonly HttpClient _client;

        // Constructor to initialize the HttpClient
        public AppointmentsIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateAppointment_ReturnsCreated()
        {
            // Set up the test appointment data
            var appointment = new Appoinment
            {
                PatientName = "Steven Douglas",
                PatientContact = "stevend@gmail.com",
                ScheduledAt = DateTime.UtcNow.AddDays(2),
                ClinicId = "ClinicJ1"
            };

            // Send POST request to create a new appointment and verify the response
            var response = await _client.PostAsJsonAsync("/api/appointments", appointment);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetAppointments_ReturnsSuccess()
        {
            // Send GET request to retrieve all appointments and verify the response
            var response = await _client.GetAsync("/api/appointments");
            response.EnsureSuccessStatusCode();

            var result await response.Content.ReadAsStringAsync();
            Assert.NotNull(result);
        }
    }
}