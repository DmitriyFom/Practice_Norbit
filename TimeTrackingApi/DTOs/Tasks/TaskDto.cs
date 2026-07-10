namespace TimeTrackingApi.DTOs.Tasks;

public record TaskDto(Guid Id, string Name, Guid ProjectId, string ProjectName, bool IsActive, DateTime CreatedAt);
public record CreateTaskRequest(string Name, Guid ProjectId);
public record UpdateTaskRequest(string Name, bool IsActive);
