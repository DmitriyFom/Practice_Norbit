using Microsoft.AspNetCore.Mvc;
using TimeTrackingApi.DTOs.Tasks;
using TimeTrackingApi.Services;

namespace TimeTrackingApi.Controllers;

/// <summary>
/// Контроллер для управления задачами.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    /// <summary>
    /// Получить список всех задач.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    /// <summary>
    /// Получить список активных задач (для выбора при создании проводки).
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActive()
    {
        return Ok(await _service.GetActiveAsync());
    }

    /// <summary>
    /// Получить задачи конкретного проекта.
    /// </summary>
    [HttpGet("by-project/{projectId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(Guid projectId)
    {
        return Ok(await _service.GetByProjectIdAsync(projectId));
    }

    /// <summary>
    /// Получить задачу по ID.
    /// </summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await _service.GetByIdAsync(id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    /// <summary>
    /// Создать новую задачу.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskRequest request)
    {
        try
        {
            var task = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Обновить задачу.
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskRequest request)
    {
        var result = await _service.UpdateAsync(id, request);
        if (!result) return NotFound();
        return NoContent();
    }

    /// <summary>
    /// Удалить задачу.
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}