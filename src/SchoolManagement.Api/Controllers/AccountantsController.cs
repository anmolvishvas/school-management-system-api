using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagement.Application.Accountants;
using SchoolManagement.Application.Accountants.Dtos;

namespace SchoolManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountantsController : ControllerBase
{
    private readonly IAccountantService _service;

    public AccountantsController(IAccountantService service)
    {
        _service = service;
    }

    [Authorize(Roles = "Admin,Accountant")]
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] bool? activeOnly = true, CancellationToken cancellationToken = default)
        => Ok(await _service.GetAccountantsAsync(activeOnly, cancellationToken));

    [Authorize(Roles = "Admin,Accountant")]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken = default)
    {
        var row = await _service.GetAccountantByIdAsync(id, cancellationToken);
        if (row == null) return NotFound();
        return Ok(row);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountantDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var created = await _service.CreateAccountantAsync(dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountantDto dto, CancellationToken cancellationToken = default)
    {
        try
        {
            var updated = await _service.UpdateAccountantAsync(id, dto, cancellationToken);
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
        var deleted = await _service.DeleteAccountantAsync(id, cancellationToken);
        if (!deleted) return NotFound();
        return Ok(new { message = "Deleted successfully" });
    }
}
