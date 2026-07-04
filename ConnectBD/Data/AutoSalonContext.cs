using Microsoft.EntityFrameworkCore;
using AutoSalonCrud.Entities;

namespace AutoSalonCrud.Data;

public class AutoSalonContext : DbContext
{
    public DbSet<Brand> Brands { get; set; } = null!;
    public DbSet<Model> Models { get; set; } = null!;
    public DbSet<Car> Cars { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<Service> Services { get; set; } = null!;
    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<SalesService> SalesServices { get; set; } = null!;

    public AutoSalonContext(DbContextOptions<AutoSalonContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AutoSalonContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}