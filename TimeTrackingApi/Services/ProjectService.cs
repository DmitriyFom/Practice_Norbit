using Microsoft.EntityFrameworkCore;
using TimeTrackingApi.Data;
using TimeTrackingApi.DTOs.Projects;
using TimeTrackingApi.Entities;

namespace TimeTrackingApi.Services;

public class ProjectService : IProjectService
{
    private readonly TimeTrackingContext _context;

    public ProjectService(TimeTrackingContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync()
    {
        return await _context.Projects
            .OrderBy(p => p.Name)
            .Select(p => new ProjectDto(p.Id, p.Name, p.Code, p.IsActive, p.CreatedAt))
            .ToListAsync();
    }

    public async Task<ProjectDto?> GetByIdAsync(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return null;
        return new ProjectDto(project.Id, project.Name, project.Code, project.IsActive, project.CreatedAt);
    }

    public async Task<ProjectDto> CreateAsync(CreateProjectRequest request)
    {
        // Проверка уникальности кода
        if (await _context.Projects.AnyAsync(p => p.Code == request.Code))
        {
            throw new InvalidOperationException($"Проект с кодом '{request.Code}' уже существует.");
        }

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Code = request.Code,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return new ProjectDto(project.Id, project.Name, project.Code, project.IsActive, project.CreatedAt);
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateProjectRequest request)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return false;

        // Проверка уникальности кода 
        if (await _context.Projects.AnyAsync(p => p.Code == request.Code && p.Id != id))
        {
            throw new InvalidOperationException($"Проект с кодом '{request.Code}' уже существует.");
        }

        project.Name = request.Name;
        project.Code = request.Code;
        project.IsActive = request.IsActive;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var project = await _context.Projects.FindAsync(id);
        if (project == null) return false;

        // Проверка наличия задач
        if (await _context.Tasks.AnyAsync(t => t.ProjectId == id))
        {
            throw new InvalidOperationException("Нельзя удалить проект с существующими задачами.");
        }

        _context.Projects.Remove(project);
        await _context.SaveChangesAsync();
        return true;
    }
}