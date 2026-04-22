using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Courses;
using SchoolManagement.Application.Courses.Dtos;

namespace SchoolManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly ICourseService _service;

    public CoursesController(ICourseService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] string? search = null, [FromQuery] bool? activeOnly = null, CancellationToken cancellationToken = default)
        => Ok(await _service.GetPagedCoursesAsync(page, pageSize, search, activeOnly, cancellationToken));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var row = await _service.GetCourseByIdAsync(id, cancellationToken);
        if (row == null) return NotFound();
        return Ok(row);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _service.CreateCourseAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCourseDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _service.UpdateCourseAsync(id, dto, cancellationToken);
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
        var deleted = await _service.DeleteCourseAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return Ok(new { message = "Deleted successfully" });
    }

    [HttpGet("{courseId:int}/sections")]
    public async Task<IActionResult> GetSections(int courseId, [FromQuery] bool? activeOnly = null, CancellationToken cancellationToken = default)
        => Ok(await _service.GetSectionsAsync(courseId, activeOnly, cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpPost("{courseId:int}/sections")]
    public async Task<IActionResult> CreateSection(int courseId, [FromBody] CreateCourseSectionDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _service.CreateSectionAsync(courseId, dto, cancellationToken);
            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("sections/{sectionId:int}")]
    public async Task<IActionResult> UpdateSection(int sectionId, [FromBody] UpdateCourseSectionDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _service.UpdateSectionAsync(sectionId, dto, cancellationToken);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("sections/{sectionId:int}")]
    public async Task<IActionResult> DeleteSection(int sectionId, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteSectionAsync(sectionId, cancellationToken);
        if (!deleted) return NotFound();
        return Ok(new { message = "Deleted successfully" });
    }

    [HttpGet("{courseId:int}/subjects")]
    public async Task<IActionResult> GetSubjects(int courseId, [FromQuery] bool? activeOnly = null, CancellationToken cancellationToken = default)
        => Ok(await _service.GetCourseSubjectsAsync(courseId, activeOnly, cancellationToken));

    [Authorize(Roles = "Admin")]
    [HttpPost("{courseId:int}/subjects")]
    public async Task<IActionResult> CreateSubject(int courseId, [FromBody] CreateCourseSubjectDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _service.CreateCourseSubjectAsync(courseId, dto, cancellationToken);
            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("subjects/{id:int}")]
    public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateCourseSubjectDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var updated = await _service.UpdateCourseSubjectAsync(id, dto, cancellationToken);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("subjects/{id:int}")]
    public async Task<IActionResult> DeleteSubject(int id, CancellationToken cancellationToken)
    {
        var deleted = await _service.DeleteCourseSubjectAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return Ok(new { message = "Deleted successfully" });
    }
}
