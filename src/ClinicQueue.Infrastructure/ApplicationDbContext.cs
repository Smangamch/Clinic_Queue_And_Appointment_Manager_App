public class ApplicationDbContext : DBContext{

    // A constructor for the DBContextOptions to be passed in and forwarded to the base DBContext class
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {}
        // Represents the Appointments table in the database 
        // Each row in the table will map to an Appointment object in C# 
        public DbSet<Appointment> Appointments { get; set; } = null!;


}