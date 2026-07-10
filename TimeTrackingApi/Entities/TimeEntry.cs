namespace TimeTrackingApi.Entities;

/// <summary>
/// Проводка рабочего времени.
/// </summary>
public class TimeEntry
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid EmployeeId { get; set; }
    public DateOnly EntryDate { get; set; }  
    public decimal Hours { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ProjectTask? Task { get; set; }
    public Employee? Employee { get; set; }
}