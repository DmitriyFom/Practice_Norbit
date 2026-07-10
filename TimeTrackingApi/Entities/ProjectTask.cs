namespace TimeTrackingApi.Entities;

/// <summary>
/// Задача в рамках проекта.
/// </summary>
public class ProjectTask
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid ProjectId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Project? Project { get; set; }
    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}