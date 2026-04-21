using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Students;
using SchoolManagement.Application.Students.Dtos;

namespace SchoolManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _students;

    public StudentsController(IStudentService students)
    {
        _students = students;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string sortBy = "id",
        [FromQuery] string order = "asc",
        [FromQuery] string? className = null,
        [FromQuery] string? section = null,
        [FromQuery] bool? activeOnly = null,
        CancellationToken cancellationToken = default)
    {
        var result = await _students.GetPagedAsync(
            page,
            pageSize,
            search,
            sortBy,
            order,
            className,
            section,
            activeOnly,
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var student = await _students.GetByIdAsync(id, cancellationToken);
        if (student == null)
            return NotFound();

        return Ok(student);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto dto, CancellationToken cancellationToken)
    {
        var created = await _students.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateStudentDto dto, CancellationToken cancellationToken)
    {
        var updated = await _students.UpdateAsync(id, dto, cancellationToken);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _students.DeleteAsync(id, cancellationToken);
        if (!deleted)
            return NotFound();

        return Ok(new { message = "Deleted successfully" });
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("stats")]
    public async Task<IActionResult> Stats(CancellationToken cancellationToken)
    {
        var stats = await _students.GetStatsAsync(cancellationToken);
        return Ok(stats);
    }
}
