using AutoMapper;
using SchoolManagement.Application.Accountants.Dtos;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Accountants;

public class AccountantService : IAccountantService
{
    private readonly IAccountantRepository _accountants;
    private readonly IUserRepository _users;
    private readonly IMapper _mapper;

    public AccountantService(IAccountantRepository accountants, IUserRepository users, IMapper mapper)
    {
        _accountants = accountants;
        _users = users;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<AccountantDto>> GetAccountantsAsync(bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var rows = await _accountants.GetAllAsync(activeOnly, cancellationToken);
        return _mapper.Map<IReadOnlyList<AccountantDto>>(rows);
    }

    public async Task<AccountantDto?> GetAccountantByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _accountants.GetByIdAsync(id, cancellationToken);
        return row == null ? null : _mapper.Map<AccountantDto>(row);
    }

    public async Task<AccountantDto> CreateAccountantAsync(CreateAccountantDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(dto.UserId, cancellationToken) ?? throw new InvalidOperationException("User not found.");
        if (!string.Equals(user.Role, ApplicationRoles.Accountant, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(user.Role, ApplicationRoles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("User role must be Accountant (or Admin).");
        }

        var existing = await _accountants.GetByUserIdAsync(dto.UserId, cancellationToken);
        if (existing != null) throw new InvalidOperationException("Accountant profile already exists for this user.");

        var row = new Accountant
        {
            UserId = dto.UserId,
            FullName = dto.FullName.Trim(),
            Phone = dto.Phone,
            Email = dto.Email,
            IsActive = dto.IsActive
        };

        await _accountants.AddAsync(row, cancellationToken);
        var created = await _accountants.GetByIdAsync(row.Id, cancellationToken) ?? row;
        return _mapper.Map<AccountantDto>(created);
    }

    public async Task<AccountantDto?> UpdateAccountantAsync(int id, UpdateAccountantDto dto, CancellationToken cancellationToken = default)
    {
        var row = await _accountants.GetByIdAsync(id, cancellationToken);
        if (row == null) return null;

        row.FullName = dto.FullName.Trim();
        row.Phone = dto.Phone;
        row.Email = dto.Email;
        row.IsActive = dto.IsActive;
        await _accountants.UpdateAsync(row, cancellationToken);
        var updated = await _accountants.GetByIdAsync(id, cancellationToken) ?? row;
        return _mapper.Map<AccountantDto>(updated);
    }

    public Task<bool> DeleteAccountantAsync(int id, CancellationToken cancellationToken = default)
        => _accountants.DeleteAsync(id, cancellationToken);
}
