using TimeTrackingApi.DTOs.TimeEntries;

namespace TimeTrackingApi.Services;

public interface ITimeEntryService
{
    Task<IEnumerable<TimeEntryDto>> GetAllAsync();
    Task<IEnumerable<TimeEntryDto>> GetByDateAsync(DateOnly date);
    Task<IEnumerable<TimeEntryDto>> GetByMonthAsync(int year, int month);
    Task<IEnumerable<TimeEntryDto>> GetByEmployeeAndDateAsync(Guid employeeId, DateOnly date);
    Task<TimeEntryDto?> GetByIdAsync(Guid id);
    Task<TimeEntryDto> CreateAsync(CreateTimeEntryRequest request);
    Task<bool> UpdateAsync(Guid id, UpdateTimeEntryRequest request);
    Task<bool> DeleteAsync(Guid id);
    Task<DailySummaryDto> GetDailySummaryAsync(Guid employeeId, DateOnly date);
}