using EntityFrameworkCore;
using ClinicQueue.Infrastructure;

var builder = WebApplications.CreateBuilder();


// Configure the database context to use SQLite by registering the ApplicationDbContext with the dependency injection container
builder.Services.AddDbContext<ApplicationDbContext>(
    options => options.UseSQlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=clinicqueue.db"
    )
);

//Add services to the container
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline for the application
app.UseAuthorization();
app.MapControllers();