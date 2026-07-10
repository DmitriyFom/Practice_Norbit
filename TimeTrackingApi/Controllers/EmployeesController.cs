using Microsoft.AspNetCore.Mvc;
using TimeTrackingApi.DTOs.Employees;
using TimeTrackingApi.Services;

namespace TimeTrackingApi.Controllers;
/// <summary>
/// Контроллер сотрудники.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        try
        {
            var employee = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetAll), new { id = employee.Id }, employee);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}