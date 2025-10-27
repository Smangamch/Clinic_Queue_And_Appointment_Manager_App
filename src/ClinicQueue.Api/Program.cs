using Microsoft.EntityFrameworkCore;
using ClinicQueue.Infrastructure;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder();


// Configure the database context to use SQLite by registering the ApplicationDbContext with the dependency injection container
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=clinicqueue.db" // Fallback connection string if not found in configuration
    )
);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Enable middleware to serve generated Swagger as a JSON endpoint and the Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception handling middleware to catch unhandled exceptions and return a JSON error response
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var errorResponse = new
        {
            message = "An unexpected error occurred.",
            detail = ex.Message
        };

        var jsonResponse = System.Text.Json.JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(jsonResponse);
    }
});

// Configure the HTTP request pipeline for the application
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();

public partial class Program { } // For integration testing purposes