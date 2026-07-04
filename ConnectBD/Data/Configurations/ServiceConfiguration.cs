using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoSalonCrud.Entities;

namespace AutoSalonCrud.Data.Configurations;

public class ServiceConfiguration : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.ToTable("Services");
        builder.HasKey(s => s.ServiceId);
        builder.Property(s => s.ServiceId).HasColumnName("ServiceID");
        builder.Property(s => s.ServiceName).HasColumnName("ServiceName").IsRequired().HasMaxLength(100);
        builder.Property(s => s.Price).HasColumnName("Price").HasColumnType("decimal(10,2)");
        builder.Property(s => s.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
        builder.Property(s => s.Description).HasColumnName("Description").HasMaxLength(500);
    }
}