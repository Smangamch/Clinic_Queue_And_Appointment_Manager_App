using Microsoft.EntityFrameworkCore;
using ClinicQueue.Domain.Entities;

// The ApplicationDbContext class inherits from the DbContext class provided by Entity Framework Core,
// It acts as bridge between the C# domain models (which is Appointment in this case) and the underlying database
// In this class, It inherits from the DBContext which provides the necessary functionality to interact with the database
public class ApplicationDbContext : DbContext{

    // A constructor for the DBContextOptions to be passed in and forwarded to the base DBContext class
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
    // Represents the Appointments table in the database, Each row in the table will map to an Appointment object in C# 
    public DbSet<Appointment> Appointments { get; set; } = null!;
    // Configuration for entities that the EF core cannot figure out automatically
    protected override void OnModelCreating(ModelBuilder modelBuilder){
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Appointment>() // This ensures that no two appointments can be scheduled at the same time for the same clinic
            .HasIndex(appointment => new { appointment.ClinicId, appointment.ScheduledAt })
            .IsUnique();
    }
}