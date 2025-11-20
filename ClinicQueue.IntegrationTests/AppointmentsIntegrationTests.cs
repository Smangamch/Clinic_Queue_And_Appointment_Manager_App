using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using ClinicQueue.Domain.Entities;

namespace ClinicQueue.IntegrationTests
{
    public class AppointmentsIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        // Constructor that initializes the HttpClient for making requests to the test server
        public AppointmentsIntegrationTests(CustomWebApplicationFactory factory)
        {
            // This ensures _client is properly initialized for all tests
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task CreateAppointment_ReturnsCreated()
        {
            var appointment = new Appointment
            {
                PatientName = "Steven Douglas",
                PatientContact = "stevend@gmail.com",
                ScheduledAt = DateTime.UtcNow.AddDays(2),
                ClinicId = "ClinicJ1"
            };

            // Act - POST the appointment
            var response = await _client.PostAsJsonAsync("/api/appointments", appointment);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {content}");

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Unexpected status code: {response.StatusCode}"
            );
        }

        [Fact]
        public async Task GetAppointments_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/api/appointments");
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response Content: {result}");

            // Assert
            Assert.True(
                response.StatusCode == HttpStatusCode.OK,
                $"Unexpected status code: {response.StatusCode}"
            );
        } 

        [Fact]
        public async Task GetAppointments_Endpoint_ShouldExist()
        {
            // Act
            var response = await _client.GetAsync("/api/appointments");

            // Assert
            Assert.NotEqual(HttpStatusCode.OK, response.StatusCode);
            Console.WriteLine($"GET /api/appointments returned status code: {response.StatusCode}");
        }

        /* var debug = await _client.GetStringAsync("/debug/routes");
        Console.WriteLine(debug); */

    }
}


