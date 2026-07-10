using Microsoft.EntityFrameworkCore;
using TimeTrackingApi.Data;
using TimeTrackingApi.DTOs.Tasks;
using TimeTrackingApi.Entities;

namespace TimeTrackingApi.Services;

public class TaskService : ITaskService
{
    private readonly TimeTrackingContext _context;

    public TaskService(TimeTrackingContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TaskDto>> GetAllAsync()
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .OrderBy(t => t.Name)
            .Select(t => new TaskDto(t.Id, t.Name, t.ProjectId, t.Project!.Name, t.IsActive, t.CreatedAt))
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskDto>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.Name)
            .Select(t => new TaskDto(t.Id, t.Name, t.ProjectId, t.Project!.Name, t.IsActive, t.CreatedAt))
            .ToListAsync();
    }

    public async Task<IEnumerable<TaskDto>> GetActiveAsync()
    {
        return await _context.Tasks
            .Include(t => t.Project)
            .Where(t => t.IsActive && t.Project!.IsActive)
            .OrderBy(t => t.Name)
            .Select(t => new TaskDto(t.Id, t.Name, t.ProjectId, t.Project!.Name, t.IsActive, t.CreatedAt))
            .ToListAsync();
    }

    public async Task<TaskDto?> GetByIdAsync(Guid id)
    {
        var task = await _context.Tasks
            .Include(t => t.Project)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (task == null) return null;
        return new TaskDto(task.Id, task.Name, task.ProjectId, task.Project!.Name, task.IsActive, task.CreatedAt);
    }

    public async Task<TaskDto> CreateAsync(CreateTaskRequest request)
    {
        var project = await _context.Projects.FindAsync(request.ProjectId);
        if (project == null)
            throw new InvalidOperationException($"Проект с ID {request.ProjectId} не найден.");

        var task = new ProjectTask
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ProjectId = request.ProjectId,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();

        return new TaskDto(task.Id, task.Name, task.ProjectId, project.Name, task.IsActive, task.CreatedAt);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTaskRequest request)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        task.Name = request.Name;
        task.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task == null) return false;

        // Проверка наличия проводок
        if (await _context.TimeEntries.AnyAsync(te => te.TaskId == id))
        {
            throw new InvalidOperationException("Нельзя удалить задачу с существующими проводками.");
        }

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
}