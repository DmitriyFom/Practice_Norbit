using TimeTrackingApi.DTOs.Employees;

namespace TimeTrackingApi.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(Guid id);
    Task<EmployeeDto> CreateAsync(CreateEmployeeRequest request);
}