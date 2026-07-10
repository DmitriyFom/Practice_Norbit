namespace TimeTrackingApi.DTOs.Projects;

public record ProjectDto(Guid Id, string Name, string Code, bool IsActive, DateTime CreatedAt);
public record CreateProjectRequest(string Name, string Code);
public record UpdateProjectRequest(string Name, string Code, bool IsActive);