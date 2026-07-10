using Microsoft.AspNetCore.Mvc;
using TimeTrackingApi.DTOs.TimeEntries;
using TimeTrackingApi.Services;

namespace TimeTrackingApi.Controllers;

/// <summary>
/// Контроллер для управления проводками рабочего времени.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TimeEntriesController : ControllerBase
{
    private readonly ITimeEntryService _service;

    public TimeEntriesController(ITimeEntryService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить все проводки за всё время.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TimeEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    /// <summary>
    /// Получить проводки за конкретную дату.
    /// </summary>
    [HttpGet("by-date/{date:datetime}")]
    [ProducesResponseType(typeof(IEnumerable<TimeEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByDate(DateOnly date)
    {
        return Ok(await _service.GetByDateAsync(date));
    }

    /// <summary>
    /// Получить проводки за месяц.
    /// </summary>
    [HttpGet("by-month/{year:int}/{month:int}")]
    [ProducesResponseType(typeof(IEnumerable<TimeEntryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByMonth(int year, int month)
    {
        if (month < 1 || month > 12)
            return BadRequest(new { message = "Месяц должен быть от 1 до 12." });
        return Ok(await _service.GetByMonthAsync(year, month));
    }

    /// <summary>
    /// Получить сводку по часам за день (с цветным стикером).
    /// </summary>
    [HttpGet("daily-summary/{employeeId:guid}/{date:datetime}")]
    [ProducesResponseType(typeof(DailySummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDailySummary(Guid employeeId, DateOnly date)
    {
        return Ok(await _service.GetDailySummaryAsync(employeeId, date));
    }

    /// <summary>
    /// Получить проводку по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TimeEntryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var entry = await _service.GetByIdAsync(id);
        if (entry == null) return NotFound();
        return Ok(entry);
    }

    /// <summary>
    /// Создать новую проводку.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TimeEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTimeEntryRequest request)
    {
        try
        {
            var entry = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Обновить проводку.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTimeEntryRequest request)
    {
        try
        {
            var result = await _service.UpdateAsync(id, request);
            if (!result) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Удалить проводку.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}