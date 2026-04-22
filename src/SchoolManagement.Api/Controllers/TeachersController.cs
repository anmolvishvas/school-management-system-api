using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Teachers;
using SchoolManagement.Application.Teachers.Dtos;

namespace SchoolManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _service;

    public TeachersController(ITeacherService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin,Teacher,Accountant")]
    [HttpGet]
    public async Task<IActionResult> GetTeachers([FromQuery] bool? activeOnly = true, CancellationToken cancellationToken = default)
        => Ok(await _service.GetTeachersAsync(activeOnly, cancellationToken));

    [Authorize(Roles = "Admin,Teacher,Accountant")]
    [HttpGet("{id:int}/teaching-plan")]
    public async Task<IActionResult> GetTeachingPlan(
        int id,
        [FromQuery] DateOnly? fromDate = null,
        [FromQuery] DateOnly? toDate = null,
        [FromQuery] bool? activeOnly = true,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var plan = await _service.GetTeachingPlanAsync(id, fromDate, toDate, activeOnly, cancellationToken);
            if (plan == null) return NotFound();
            return Ok(plan);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateTeacher([FromBody] CreateTeacherDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _service.CreateTeacherAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetTeachers), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateTeacher(int id, [FromBody] UpdateTeacherDto dto, CancellationToken cancellationToken)
    {
        var updated = await _service.UpdateTeacherAsync(id, dto, cancellationToken);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteTeacher(int id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteTeacherAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return Ok(new { message = "Deleted successfully" });
    }

    [Authorize(Roles = "Admin,Teacher,Accountant")]
    [HttpGet("allocations")]
    public async Task<IActionResult> GetAllocations([FromQuery] int page = 1, [FromQuery] int pageSize = 30, [FromQuery] int? teacherId = null, [FromQuery] int? subjectId = null, [FromQuery] string? className = null, [FromQuery] string? section = null, [FromQuery] bool? activeOnly = true, CancellationToken cancellationToken = default)
        => Ok(await _service.GetAllocationsAsync(page, pageSize, teacherId, subjectId, className, section, activeOnly, cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpPost("allocations")]
    public async Task<IActionResult> CreateAllocation([FromBody] CreateTeacherAllocationDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _service.CreateAllocationAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetAllocations), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("allocations/{id:int}")]
    public async Task<IActionResult> UpdateAllocation(int id, [FromBody] UpdateTeacherAllocationDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _service.UpdateAllocationAsync(id, dto, cancellationToken);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("allocations/{id:int}")]
    public async Task<IActionResult> DeleteAllocation(int id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteAllocationAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return Ok(new { message = "Deleted successfully" });
    }
}
