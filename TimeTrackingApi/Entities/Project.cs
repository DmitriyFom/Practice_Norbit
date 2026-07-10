namespace TimeTrackingApi.Entities;

/// <summary>
/// Проект компании.
/// </summary>
public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}