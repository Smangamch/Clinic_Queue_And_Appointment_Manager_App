using Microsoft.EntityFrameworkCore;
using ClinicQueue.Infrastructure;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure the database context to use SQLite by registering the ApplicationDbContext with the dependency injection container
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=clinicqueue.db" // Fallback connection string if not found in configuration
    )
);

builder.Services.AddMemoryCache();

var app = builder.Build();

// Enable middleware to serve generated Swagger as a JSON endpoint and the Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



// Configure the HTTP request pipeline for the application
// only enable HTTPS redirection in Production to avoid test host redirect issues
app.UseAuthorization();
app.MapControllers();
app.Run();
app.MapGet("/debug/routes", (EndpointDataSource es) =>
    es.Endpoints.Select(e => e.DisplayName).ToList());


public partial class Program { } // For integration testing purposes