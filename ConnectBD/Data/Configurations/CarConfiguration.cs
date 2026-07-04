using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoSalonCrud.Entities;

namespace AutoSalonCrud.Data.Configurations;

public class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.ToTable("Cars");
        builder.HasKey(c => c.CarId);
        builder.Property(c => c.CarId).HasColumnName("CarID");
        builder.Property(c => c.VIN).HasColumnName("VIN").IsRequired().HasMaxLength(17);
        builder.Property(c => c.ModelId).HasColumnName("ModelID");
        builder.Property(c => c.Color).HasColumnName("Color").IsRequired().HasMaxLength(30);
        builder.Property(c => c.YearOfManufacture).HasColumnName("YearOfManufacture");
        builder.Property(c => c.Mileage).HasColumnName("Mileage");
        builder.Property(c => c.Price).HasColumnName("Price").HasColumnType("decimal(12,2)");
        builder.Property(c => c.IsSold).HasColumnName("IsSold").HasDefaultValue(false);
        builder.Property(c => c.ArrivalDate).HasColumnName("ArrivalDate").HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");
        builder.Property(c => c.DiscountPercent).HasColumnName("DiscountPercent").HasColumnType("decimal(4,2)");

        builder.HasIndex(c => c.VIN).IsUnique().HasDatabaseName("UX_Cars_VIN");

        builder.HasOne(c => c.Model)
            .WithMany(m => m.Cars)
            .HasForeignKey(c => c.ModelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(c => c.FinalPrice);
    }
}