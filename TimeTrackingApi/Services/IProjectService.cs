using TimeTrackingApi.DTOs.Projects;

namespace TimeTrackingApi.Services;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllAsync();
    Task<ProjectDto?> GetByIdAsync(Guid id);
    Task<ProjectDto> CreateAsync(CreateProjectRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateProjectRequest request);
    Task<bool> DeleteAsync(Guid id);
}