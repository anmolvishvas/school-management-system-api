using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Common.Interfaces;

public interface ITeacherAllocationRepository
{
    Task<PagedResult<TeacherClassSubjectAllocation>> GetPagedAsync(
        int page,
        int pageSize,
        int? teacherId,
        int? subjectId,
        string? className,
        string? section,
        bool? activeOnly,
        CancellationToken cancellationToken = default);

    Task<TeacherClassSubjectAllocation?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<TeacherClassSubjectAllocation>> ListByTeacherAsync(int teacherId, bool? activeOnly, CancellationToken cancellationToken = default);
    Task<TeacherClassSubjectAllocation?> GetActiveByClassSectionSubjectAsync(string className, string section, int subjectId, CancellationToken cancellationToken = default);
    Task<bool> ExistsDuplicateAsync(int teacherId, int subjectId, string className, string section, int? exceptId, CancellationToken cancellationToken = default);
    Task AddAsync(TeacherClassSubjectAllocation row, CancellationToken cancellationToken = default);
    Task UpdateAsync(TeacherClassSubjectAllocation row, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
