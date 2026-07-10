using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TimeTrackingApi.Entities;

namespace TimeTrackingApi.Data.Configurations;

public class TimeEntryConfiguration : IEntityTypeConfiguration<TimeEntry>
{
    public void Configure(EntityTypeBuilder<TimeEntry> builder)
    {
        builder.ToTable("TimeEntries");
        builder.HasKey(te => te.Id);
        builder.Property(te => te.Hours).IsRequired();
        builder.Property(te => te.Description).HasMaxLength(500);

        builder.HasOne(te => te.Task)
            .WithMany(t => t.TimeEntries)
            .HasForeignKey(te => te.TaskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(te => te.Employee)
            .WithMany(e => e.TimeEntries)
            .HasForeignKey(te => te.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(te => new { te.EmployeeId, te.EntryDate });
    }
}