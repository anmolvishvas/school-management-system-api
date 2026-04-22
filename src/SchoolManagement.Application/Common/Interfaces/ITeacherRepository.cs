using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Common.Interfaces;

public interface ITeacherRepository
{
    Task<IReadOnlyList<Teacher>> GetAllAsync(bool? activeOnly, CancellationToken cancellationToken = default);
    Task<Teacher?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Teacher?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default);
    Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default);
    Task UpdateAsync(Teacher teacher, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
