using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Common.Interfaces;

public interface IStudentRepository
{
    Task<PagedResult<Student>> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string sortBy,
        string order,
        string? className,
        string? section,
        bool? activeOnly,
        CancellationToken cancellationToken = default);

    Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Student>> ListByIdsAsync(IReadOnlyCollection<int> ids, CancellationToken cancellationToken = default);
    Task AddAsync(Student student, CancellationToken cancellationToken = default);
    Task UpdateAsync(Student student, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LabelCountDto>> CountByClassAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LabelCountDto>> CountBySectionAsync(CancellationToken cancellationToken = default);
}
