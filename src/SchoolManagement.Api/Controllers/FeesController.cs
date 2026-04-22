using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Fees;
using SchoolManagement.Application.Fees.Dtos;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class FeesController : ControllerBase
{
    private readonly IFeeService _service;

    public FeesController(IFeeService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin,Accountant")]
    [HttpGet("invoices")]
    public async Task<IActionResult> GetInvoices(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 30,
        [FromQuery] int? studentId = null,
        [FromQuery] FeeInvoiceStatus? status = null,
        [FromQuery] string? className = null,
        [FromQuery] string? section = null,
        [FromQuery] DateOnly? dueFrom = null,
        [FromQuery] DateOnly? dueTo = null,
        CancellationToken cancellationToken = default)
        => Ok(await _service.GetInvoicesAsync(page, pageSize, studentId, status, className, section, dueFrom, dueTo, cancellationToken));

    [Authorize(Roles = "Admin,Accountant")]
    [HttpGet("invoices/{id:int}")]
    public async Task<IActionResult> GetInvoiceById(int id, CancellationToken cancellationToken = default)
    {
        var row = await _service.GetInvoiceByIdAsync(id, cancellationToken);
        if (row == null) return NotFound();
        return Ok(row);
    }

    [Authorize(Roles = "Admin,Accountant")]
    [HttpPost("invoices")]
    public async Task<IActionResult> CreateInvoice([FromBody] CreateFeeInvoiceDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var created = await _service.CreateInvoiceAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetInvoiceById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Accountant")]
    [HttpPut("invoices/{id:int}")]
    public async Task<IActionResult> UpdateInvoice(int id, [FromBody] UpdateFeeInvoiceDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var updated = await _service.UpdateInvoiceAsync(id, dto, cancellationToken);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Accountant")]
    [HttpDelete("invoices/{id:int}")]
    public async Task<IActionResult> DeleteInvoice(int id, CancellationToken cancellationToken = default)
    {
        var deleted = await _service.DeleteInvoiceAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return Ok(new { message = "Deleted successfully" });
    }

    [Authorize(Roles = "Admin,Accountant")]
    [HttpGet("invoices/{invoiceId:int}/payments")]
    public async Task<IActionResult> GetPayments(int invoiceId, CancellationToken cancellationToken = default)
    {
        try
        {
            return Ok(await _service.GetPaymentsAsync(invoiceId, cancellationToken));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Accountant")]
    [HttpPost("invoices/{invoiceId:int}/payments")]
    public async Task<IActionResult> AddPayment(int invoiceId, [FromBody] CreateFeePaymentDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var created = await _service.AddPaymentAsync(invoiceId, dto, TryGetUserId(), cancellationToken);
            return Ok(created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin,Accountant")]
    [HttpGet("reports/summary")]
    public async Task<IActionResult> GetSummary([FromQuery] DateOnly? from = null, [FromQuery] DateOnly? to = null, [FromQuery] string? className = null, [FromQuery] string? section = null, CancellationToken cancellationToken = default)
        => Ok(await _service.GetSummaryAsync(from, to, className, section, cancellationToken));

    private int? TryGetUserId()
    {
        var sub = User.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(sub, out var id) ? id : null;
    }
}
