namespace TimeTrackingApi.Entities;

/// <summary>
/// Сотрудник компании.
/// </summary>
public class Employee
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}