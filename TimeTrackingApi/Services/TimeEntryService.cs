using Microsoft.EntityFrameworkCore;
using TimeTrackingApi.Data;
using TimeTrackingApi.DTOs.TimeEntries;
using TimeTrackingApi.Entities;

namespace TimeTrackingApi.Services;

public class TimeEntryService : ITimeEntryService
{
    private readonly TimeTrackingContext _context;

    public TimeEntryService(TimeTrackingContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TimeEntryDto>> GetAllAsync()
    {
        return await MapToDto(_context.TimeEntries).ToListAsync();
    }

    public async Task<IEnumerable<TimeEntryDto>> GetByDateAsync(DateOnly date)
    {
        return await MapToDto(_context.TimeEntries.Where(te => te.EntryDate == date)).ToListAsync();
    }

    public async Task<IEnumerable<TimeEntryDto>> GetByMonthAsync(int year, int month)
    {
        var startDate = new DateOnly(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);

        return await MapToDto(_context.TimeEntries
            .Where(te => te.EntryDate >= startDate && te.EntryDate <= endDate))
            .ToListAsync();
    }

    public async Task<IEnumerable<TimeEntryDto>> GetByEmployeeAndDateAsync(Guid employeeId, DateOnly date)
    {
        return await MapToDto(_context.TimeEntries
            .Where(te => te.EmployeeId == employeeId && te.EntryDate == date))
            .ToListAsync();
    }

    public async Task<TimeEntryDto?> GetByIdAsync(Guid id)
    {
        return await MapToDto(_context.TimeEntries.Where(te => te.Id == id)).FirstOrDefaultAsync();
    }

    public async Task<TimeEntryDto> CreateAsync(CreateTimeEntryRequest request)
    {
        // Проверка существования задачи
        var task = await _context.Tasks.FindAsync(request.TaskId);
        if (task == null)
            throw new InvalidOperationException($"Задача с ID {request.TaskId} не найдена.");

        // Задача должна быть активной
        if (!task.IsActive)
            throw new InvalidOperationException("Нельзя создать проводку для неактивной задачи.");

        // Проверка существования сотрудника
        var employee = await _context.Employees.FindAsync(request.EmployeeId);
        if (employee == null)
            throw new InvalidOperationException($"Сотрудник с ID {request.EmployeeId} не найден.");

        // Проверка часов (0 < hours <= 24)
        if (request.Hours <= 0 || request.Hours > 24)
            throw new InvalidOperationException("Количество часов должно быть от 0 (не включая) до 24.");

        // Проверка: сумма часов за день не должна превышать 24
        var totalHoursToday = await _context.TimeEntries
            .Where(te => te.EmployeeId == request.EmployeeId && te.EntryDate == request.EntryDate)
            .SumAsync(te => (decimal?)te.Hours) ?? 0;

        if (totalHoursToday + request.Hours > 24)
        {
            throw new InvalidOperationException(
                $"Превышен лимит часов за день. Уже списано: {totalHoursToday}, " +
                $"попытка добавить: {request.Hours}, максимум: 24.");
        }

        var entry = new TimeEntry
        {
            Id = Guid.NewGuid(),
            TaskId = request.TaskId,
            EmployeeId = request.EmployeeId,
            EntryDate = request.EntryDate,
            Hours = request.Hours,
            Description = request.Description,
            CreatedAt = DateTime.Now
        };

        _context.TimeEntries.Add(entry);
        await _context.SaveChangesAsync();

        return (await GetByIdAsync(entry.Id))!;
    }

    public async Task<bool> UpdateAsync(Guid id, UpdateTimeEntryRequest request)
    {
        var entry = await _context.TimeEntries
            .Include(e => e.Task)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (entry == null) return false;
        if (request.TaskId.HasValue && request.TaskId != entry.TaskId)
        {
            if (entry.Task != null && !entry.Task.IsActive)
            {
                throw new InvalidOperationException(
                    "Нельзя изменить задачу проводки, так как исходная задача стала неактивной.");
            }

            // Проверяем, что новая задача активна
            var newTask = await _context.Tasks.FindAsync(request.TaskId);
            if (newTask == null || !newTask.IsActive)
            {
                throw new InvalidOperationException("Нельзя назначить неактивную задачу.");
            }

            entry.TaskId = request.TaskId.Value;
        }

        if (request.Hours.HasValue)
        {
            if (request.Hours <= 0 || request.Hours > 24)
                throw new InvalidOperationException("Количество часов должно быть от 0 (не включая) до 24.");

            // Проверяем лимит 24 часов за день
            var otherHours = await _context.TimeEntries
                .Where(te => te.EmployeeId == entry.EmployeeId
                          && te.EntryDate == entry.EntryDate
                          && te.Id != id)
                .SumAsync(te => (decimal?)te.Hours) ?? 0;

            if (otherHours + request.Hours > 24)
            {
                throw new InvalidOperationException(
                    $"Превышен лимит часов за день. Другие проводки: {otherHours} ч.");
            }

            entry.Hours = request.Hours.Value;
        }

        if (request.Description != null)
        {
            entry.Description = request.Description;
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var entry = await _context.TimeEntries.FindAsync(id);
        if (entry == null) return false;

        _context.TimeEntries.Remove(entry);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<DailySummaryDto> GetDailySummaryAsync(Guid employeeId, DateOnly date)
    {
        var totalHours = await _context.TimeEntries
            .Where(te => te.EmployeeId == employeeId && te.EntryDate == date)
            .SumAsync(te => (decimal?)te.Hours) ?? 0;

        string status;
        string color;

        if (totalHours < 8)
        {
            status = "Insufficient"; 
            color = "yellow";
        }
        else if (totalHours == 8)
        {
            status = "Normal";      
            color = "green";
        }
        else
        {
            status = "Excess";    
            color = "red";
        }

        return new DailySummaryDto(date, totalHours, status, color);
    }

    private IQueryable<TimeEntryDto> MapToDto(IQueryable<TimeEntry> query)
    {
        return query
            .Include(te => te.Task)
            .Include(te => te.Employee)
            .OrderByDescending(te => te.EntryDate)
            .ThenBy(te => te.CreatedAt)
            .Select(te => new TimeEntryDto(
                te.Id,
                te.TaskId,
                te.Task!.Name,
                te.EmployeeId,
                $"{te.Employee!.FirstName} {te.Employee.LastName}",
                te.EntryDate,
                te.Hours,
                te.Description,
                te.CreatedAt
            ));
    }
}