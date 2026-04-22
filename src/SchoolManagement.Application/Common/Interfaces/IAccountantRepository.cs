using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Common.Interfaces;

public interface IAccountantRepository
{
    Task<IReadOnlyList<Accountant>> GetAllAsync(bool? activeOnly, CancellationToken cancellationToken = default);
    Task<Accountant?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Accountant?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(Accountant row, CancellationToken cancellationToken = default);
    Task UpdateAsync(Accountant row, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
