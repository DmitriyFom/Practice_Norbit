using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoSalonCrud.Entities;

namespace AutoSalonCrud.Data.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        builder.HasKey(c => c.CustomerId);
        builder.Property(c => c.CustomerId).HasColumnName("CustomerID");
        builder.Property(c => c.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(50);
        builder.Property(c => c.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(50);
        builder.Property(c => c.Phone).HasColumnName("Phone").IsRequired().HasMaxLength(20);
        builder.Property(c => c.Email).HasColumnName("Email").HasMaxLength(100);
        builder.Property(c => c.IsVIP).HasColumnName("IsVIP").HasDefaultValue(false);
        builder.Property(c => c.RegistrationDate).HasColumnName("RegistrationDate")
            .HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
    }
}