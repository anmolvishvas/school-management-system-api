using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Common.Interfaces;

public interface ISubjectRepository
{
    Task<IReadOnlyList<Subject>> GetAllAsync(bool? activeOnly, CancellationToken cancellationToken = default);
    Task<Subject?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Subject?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task AddAsync(Subject subject, CancellationToken cancellationToken = default);
    Task UpdateAsync(Subject subject, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
