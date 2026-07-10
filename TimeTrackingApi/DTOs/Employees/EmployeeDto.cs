namespace TimeTrackingApi.DTOs.Employees;

public record EmployeeDto(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    bool IsActive
);

public record CreateEmployeeRequest(
    string FirstName,
    string LastName,
    string Email
);