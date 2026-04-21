using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Dashboard;

namespace SchoolManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboard;

    public DashboardController(IDashboardService dashboard)
    {
        _dashboard = dashboard;
    }

    [HttpGet("kpis")]
    public async Task<IActionResult> GetKpis(CancellationToken cancellationToken)
    {
        var kpis = await _dashboard.GetKpisAsync(cancellationToken);
        return Ok(kpis);
    }
}
