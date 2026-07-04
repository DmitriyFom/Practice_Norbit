using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AutoSalonCrud.Entities;

namespace AutoSalonCrud.Data.Configurations;

public class ModelConfiguration : IEntityTypeConfiguration<Model>
{
    public void Configure(EntityTypeBuilder<Model> builder)
    {
        builder.ToTable("Models");
        builder.HasKey(m => m.ModelId);
        builder.Property(m => m.ModelId).HasColumnName("ModelID");
        builder.Property(m => m.ModelName).HasColumnName("ModelName").IsRequired().HasMaxLength(100);
        builder.Property(m => m.BrandId).HasColumnName("BrandID");
        builder.Property(m => m.BodyType).HasColumnName("BodyType").IsRequired().HasMaxLength(50);
        builder.Property(m => m.BasePrice).HasColumnName("BasePrice").HasColumnType("decimal(12,2)");
        builder.Property(m => m.IsActive).HasColumnName("IsActive").HasDefaultValue(true);
    }
}