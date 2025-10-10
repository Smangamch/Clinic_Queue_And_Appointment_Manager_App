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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline for the application
app.UseAuthorization();
app.MapControllers();
app.UseHttpsRedirection();
app.Run();