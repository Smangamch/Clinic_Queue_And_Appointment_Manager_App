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

var app = builder.Build();

// Configure the HTTP request pipeline for the application
app.UseAuthorization();
app.MapControllers();
app.Run();