using SchoolManagement.Application.Accountants.Dtos;

namespace SchoolManagement.Application.Accountants;

public interface IAccountantService
{
    Task<IReadOnlyList<AccountantDto>> GetAccountantsAsync(bool? activeOnly, CancellationToken cancellationToken = default);
    Task<AccountantDto?> GetAccountantByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<AccountantDto> CreateAccountantAsync(CreateAccountantDto dto, CancellationToken cancellationToken = default);
    Task<AccountantDto?> UpdateAccountantAsync(int id, UpdateAccountantDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAccountantAsync(int id, CancellationToken cancellationToken = default);
}
