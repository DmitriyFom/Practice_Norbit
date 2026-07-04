using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoSalonCrud.Entities;

namespace AutoSalonCrud.Data.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");
        builder.HasKey(s => s.SaleId);
        builder.Property(s => s.SaleId).HasColumnName("SaleID");
        builder.Property(s => s.CarId).HasColumnName("CarID");
        builder.Property(s => s.CustomerId).HasColumnName("CustomerID");
        builder.Property(s => s.SaleDate).HasColumnName("SaleDate")
            .HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
        builder.Property(s => s.FinalPrice).HasColumnName("FinalPrice").HasColumnType("decimal(12,2)");
        builder.Property(s => s.PaymentMethod).HasColumnName("PaymentMethod").IsRequired().HasMaxLength(50);
        builder.Property(s => s.IsCompleted).HasColumnName("IsCompleted").HasDefaultValue(false);

        builder.HasOne(s => s.Car)
            .WithMany()
            .HasForeignKey(s => s.CarId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Customer)
            .WithMany(c => c.Sales)
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}