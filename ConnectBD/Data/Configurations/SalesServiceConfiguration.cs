using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoSalonCrud.Entities;

namespace AutoSalonCrud.Data.Configurations;

public class SalesServiceConfiguration : IEntityTypeConfiguration<SalesService>
{
    public void Configure(EntityTypeBuilder<SalesService> builder)
    {
        builder.ToTable("SalesServices");
        builder.HasKey(ss => new { ss.SaleId, ss.ServiceId });

        builder.Property(ss => ss.SaleId).HasColumnName("SaleID");
        builder.Property(ss => ss.ServiceId).HasColumnName("ServiceID");
        builder.Property(ss => ss.ServicePrice).HasColumnName("ServicePrice").HasColumnType("decimal(10,2)");

        builder.HasOne(ss => ss.Sale)
            .WithMany(s => s.SalesServices)
            .HasForeignKey(ss => ss.SaleId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ss => ss.Service)
            .WithMany(s => s.SalesServices)
            .HasForeignKey(ss => ss.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}