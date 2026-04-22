using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.PeriodAttendance;
using SchoolManagement.Application.PeriodAttendance.Dtos;

namespace SchoolManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class SubjectsController : ControllerBase
{
    private readonly IPeriodAttendanceService _subjects;

    public SubjectsController(IPeriodAttendanceService subjects)
    {
        _subjects = subjects;
    }

    [Authorize(Roles = "Admin,Teacher,Accountant")]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? activeOnly = true, CancellationToken cancellationToken = default)
        => Ok(await _subjects.GetSubjectsAsync(activeOnly, cancellationToken));

    [Authorize(Roles = "Admin,Teacher,Accountant")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var row = await _subjects.GetSubjectByIdAsync(id, cancellationToken);
        if (row == null) return NotFound();
        return Ok(row);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSubjectDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _subjects.CreateSubjectAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateSubjectDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _subjects.UpdateSubjectAsync(id, dto, cancellationToken);
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
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _subjects.DeleteSubjectAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return Ok(new { message = "Deleted successfully" });
    }
}
