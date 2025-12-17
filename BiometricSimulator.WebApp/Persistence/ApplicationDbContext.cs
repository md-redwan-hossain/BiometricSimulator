using BiometricSimulator.WebApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace BiometricSimulator.WebApp.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Employee entity
        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ProxyCode)
                .HasMaxLength(30)
                .IsRequired();

            entity.HasIndex(e => e.ProxyCode)
                .IsUnique();
        });

        // Configure ActivityLog entity
        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.HasKey(e => e.Id);

            // Configure relationship
            entity.HasOne<Employee>()
                .WithMany()
                .HasForeignKey(a => a.EmployeeId)
                .IsRequired();

            // Configure indexes
            entity.HasIndex(a => a.EmployeeId);
            entity.HasIndex(a => a.IsProcessed);
        });
    }
}