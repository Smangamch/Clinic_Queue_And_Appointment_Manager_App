using ClinicQueue.Infrastructure;
using Microsoft.EntityFrameworkCore;
using ClinicQueue.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// EF with SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=clinicqueue.db"
    )
);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy => {
            policy.WithOrigins("http://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddMemoryCache();
builder.Services.AddLogging();

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Pipeline
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowFrontend");

app.Run();

public partial class Program { }  // Required for WebApplicationFactory
