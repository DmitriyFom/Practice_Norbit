using Microsoft.EntityFrameworkCore;
using TimeTrackingApi.Entities;

namespace TimeTrackingApi.Data;

public class TimeTrackingContext : DbContext
{
    public DbSet<Project> Projects { get; set; } = null!;
    public DbSet<ProjectTask> Tasks { get; set; } = null!;
    public DbSet<TimeEntry> TimeEntries { get; set; } = null!;
    public DbSet<Employee> Employees { get; set; } = null!;

    public TimeTrackingContext(DbContextOptions<TimeTrackingContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TimeTrackingContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}