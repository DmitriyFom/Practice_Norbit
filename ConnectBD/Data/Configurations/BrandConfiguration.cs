using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoSalonCrud.Entities;

namespace AutoSalonCrud.Data.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");
        builder.HasKey(b => b.BrandId);
        builder.Property(b => b.BrandId).HasColumnName("BrandID");
        builder.Property(b => b.BrandName).HasColumnName("BrandName").IsRequired().HasMaxLength(100);
        builder.Property(b => b.Country).HasColumnName("Country").IsRequired().HasMaxLength(50);
        builder.Property(b => b.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
        builder.Property(b => b.FoundedYear).HasColumnName("FoundedYear");
        builder.Property(b => b.CreatedAt).HasColumnName("CreatedAt").HasColumnType("datetime2").HasDefaultValueSql("GETDATE()");

        builder.HasMany(b => b.Models)
            .WithOne(m => m.Brand)
            .HasForeignKey(m => m.BrandId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}