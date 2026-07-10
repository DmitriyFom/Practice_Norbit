namespace TimeTrackingApi.DTOs.TimeEntries;

public record TimeEntryDto(
    Guid Id,
    Guid TaskId,
    string TaskName,
    Guid EmployeeId,
    string EmployeeName,
    DateOnly EntryDate,
    decimal Hours,
    string Description,
    DateTime CreatedAt
);

public record CreateTimeEntryRequest(
    Guid TaskId,
    Guid EmployeeId,
    DateOnly EntryDate,
    decimal Hours,
    string Description
);

public record UpdateTimeEntryRequest(
    Guid? TaskId,
    decimal? Hours,
    string? Description
);

/// <summary>
/// DTO для визуализации стикеры.
/// </summary>
public record DailySummaryDto(
    DateOnly Date,
    decimal TotalHours,
    string Status,        
    string Color          
);