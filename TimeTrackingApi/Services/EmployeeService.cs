using Microsoft.EntityFrameworkCore;
using TimeTrackingApi.Data;
using TimeTrackingApi.DTOs.Employees;
using TimeTrackingApi.Entities;

namespace TimeTrackingApi.Services;

public class EmployeeService : IEmployeeService
{
    private readonly TimeTrackingContext _context;

    public EmployeeService(TimeTrackingContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
    {
        return await _context.Employees
            .Where(e => e.IsActive)
            .OrderBy(e => e.LastName)
            .Select(e => new EmployeeDto(e.Id, e.FirstName, e.LastName, e.Email, e.IsActive))
            .ToListAsync();
    }

    public async Task<EmployeeDto?> GetByIdAsync(Guid id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null) return null;
        return new EmployeeDto(employee.Id, employee.FirstName, employee.LastName, employee.Email, employee.IsActive);
    }

    public async Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request)
    {
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            IsActive = true
        };

        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return new EmployeeDto(employee.Id, employee.FirstName, employee.LastName, employee.Email, employee.IsActive);
    }
}