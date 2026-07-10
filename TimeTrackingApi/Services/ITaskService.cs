using TimeTrackingApi.DTOs.Tasks;

namespace TimeTrackingApi.Services;

public interface ITaskService
{
    Task<IEnumerable<TaskDto>> GetAllAsync();
    Task<IEnumerable<TaskDto>> GetByProjectIdAsync(Guid projectId);
    Task<IEnumerable<TaskDto>> GetActiveAsync();
    Task<TaskDto?> GetByIdAsync(Guid id);
    Task<TaskDto> CreateAsync(CreateTaskRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateTaskRequest request);
    Task<bool> DeleteAsync(Guid id);
}