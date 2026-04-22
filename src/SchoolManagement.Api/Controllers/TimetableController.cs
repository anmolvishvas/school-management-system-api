using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Timetables;
using SchoolManagement.Application.Timetables.Dtos;

namespace SchoolManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TimetableController : ControllerBase
{
    private readonly ITimetableService _service;

    public TimetableController(ITimetableService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? className = null,
        [FromQuery] string? section = null,
        [FromQuery] DayOfWeek? dayOfWeek = null,
        [FromQuery] int? teacherId = null,
        [FromQuery] bool? activeOnly = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _service.GetPagedAsync(page, pageSize, className, section, dayOfWeek, teacherId, activeOnly, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var row = await _service.GetByIdAsync(id, cancellationToken);
        if (row == null) return NotFound();
        return Ok(row);
    }

    [HttpGet("class-section")]
    public async Task<IActionResult> GetByClassSection([FromQuery] string className, [FromQuery] string section, [FromQuery] bool? activeOnly = true, CancellationToken cancellationToken = default)
        => Ok(await _service.GetByClassSectionAsync(className, section, activeOnly, cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTimetableEntryDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var created = await _service.CreateAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("bulk-weekly")]
    public async Task<IActionResult> BulkWeekly([FromBody] BulkUpsertWeeklyTimetableDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _service.BulkUpsertWeeklyAsync(dto, cancellationToken);
            return Ok(new { upserted = count });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTimetableEntryDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, dto, cancellationToken);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _service.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return Ok(new { message = "Deleted successfully" });
    }
}
