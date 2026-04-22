using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.PeriodAttendance;
using SchoolManagement.Application.PeriodAttendance.Dtos;

namespace SchoolManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PeriodAttendanceController : ControllerBase
{
    private readonly IPeriodAttendanceService _service;

    public PeriodAttendanceController(IPeriodAttendanceService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin,Teacher,Accountant")]
    [HttpGet]
    public async Task<IActionResult> GetPeriodAttendance([FromQuery] int page = 1, [FromQuery] int pageSize = 30, [FromQuery] int? studentId = null, [FromQuery] int? subjectId = null, [FromQuery] string? className = null, [FromQuery] string? section = null, [FromQuery] DateOnly? dateFrom = null, [FromQuery] DateOnly? dateTo = null, [FromQuery] int? hourNumber = null, [FromQuery] string sortBy = "date", [FromQuery] string order = "desc", CancellationToken cancellationToken = default)
    {
        var result = await _service.GetPeriodAttendanceAsync(page, pageSize, studentId, subjectId, className, section, dateFrom, dateTo, hourNumber, sortBy, order, cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "Admin,Teacher")]
    [HttpPost("bulk-mark")]
    public async Task<IActionResult> BulkMark([FromBody] BulkMarkPeriodAttendanceDto dto, CancellationToken cancellationToken)
    {
        try
        {
            var count = await _service.BulkMarkPeriodAsync(dto, TryGetUserId(), cancellationToken);
            return Ok(new { marked = count });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    private int? TryGetUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(sub, out var id) ? id : null;
    }
}
