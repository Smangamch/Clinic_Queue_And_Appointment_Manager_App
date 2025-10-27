using System;
using System.Net;
using System.Net.Http.Json;
using Xunit;
using ClinicQueue.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace ClinicQueue.IntegrationTests
{
    public class AppointmentsIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        // Create an HttpClient to send requests to the test server
        private readonly HttpClient _client;

        // Constructor to initialize the HttpClient
        public AppointmentsIntegrationTests(WebApplicationFactory<Program> factory)
        {
            var configuredFactory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Development");
                builder.ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.SetMinimumLevel(LogLevel.Debug);
                });
            });

            _client = configuredFactory.CreateClient();
        }

        [Fact]
        public async Task CreateAppointment_ReturnsCreated()
        {
            // Set up the test appointment data
            var appointment = new Appointment
            {
                PatientName = "Steven Douglas",
                PatientContact = "stevend@gmail.com",
                ScheduledAt = DateTime.UtcNow.AddDays(2),
                ClinicId = "ClinicJ1"
            };

            var response = await _client.PostAsJsonAsync("/api/appointments", appointment);
            Assert.True(
                response.StatusCode == HttpStatusCode.Created,
                $"Unexpected status code: {response.StatusCode}"
            );
        }
        

        [Fact]
        public async Task GetAppointments_ReturnsSuccess()
        {
            // Send GET request to retrieve all appointments and verify the response
            var response = await _client.GetAsync("/api/appointments");
            Assert.True(
                response.StatusCode == HttpStatusCode.OK,
                $"Unexpected status code: {response.StatusCode}"
            );
        }
    }
}