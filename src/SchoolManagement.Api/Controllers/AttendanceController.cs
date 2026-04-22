using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Attendance;
using SchoolManagement.Application.Attendance.Dtos;

namespace SchoolManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendance;

    public AttendanceController(IAttendanceService attendance)
    {
        _attendance = attendance;
    }

    [Authorize(Roles = "Admin,Teacher,Accountant")]
    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 20, [FromQuery] int? studentId = null, [FromQuery] string? className = null, [FromQuery] string? section = null, [FromQuery] DateOnly? dateFrom = null, [FromQuery] DateOnly? dateTo = null, [FromQuery] string sortBy = "date", [FromQuery] string order = "desc", CancellationToken cancellationToken = default)
    {
        var result = await _attendance.GetPagedAsync(page, pageSize, studentId, className, section, dateFrom, dateTo, sortBy, order, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,Teacher,Accountant")]
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateOnly from, [FromQuery] DateOnly to, [FromQuery] string? className = null, [FromQuery] string? section = null, CancellationToken cancellationToken = default)
    {
        if (to < from) return BadRequest(new { error = "'to' must be on or after 'from'." });
        var summary = await _attendance.GetSummaryAsync(from, to, className, section, cancellationToken);
        return Ok(summary);
    }

    [Authorize(Roles = "Admin,Teacher,Accountant")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var row = await _attendance.GetByIdAsync(id, cancellationToken);
        if (row == null) return NotFound();
        return Ok(row);
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAttendanceRecordDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var created = await _attendance.CreateAsync(dto, TryGetUserId(), cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            if (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
                return Conflict(new { error = ex.Message });
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost("bulk-day")]
    public async Task<IActionResult> BulkMarkDay([FromBody] BulkMarkAttendanceDayDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var count = await _attendance.BulkMarkDayAsync(dto, TryGetUserId(), cancellationToken);
            return Ok(new { marked = count });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAttendanceRecordDto dto, CancellationToken cancellationToken)
    {
        var updated = await _attendance.UpdateAsync(id, dto, cancellationToken);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await _attendance.DeleteAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return Ok(new { message = "Deleted successfully" });
    }

    private int? TryGetUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(sub, out var id) ? id : null;
    }
}
