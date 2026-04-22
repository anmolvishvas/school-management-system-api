using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Teachers.Dtos;

namespace SchoolManagement.Application.Teachers;

public interface ITeacherService
{
    Task<IReadOnlyList<TeacherDto>> GetTeachersAsync(bool? activeOnly, CancellationToken cancellationToken = default);
    Task<TeacherDto?> GetTeacherByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto dto, CancellationToken cancellationToken = default);
    Task<TeacherDto?> UpdateTeacherAsync(int id, UpdateTeacherDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteTeacherAsync(int id, CancellationToken cancellationToken = default);

    Task<PagedResult<TeacherAllocationDto>> GetAllocationsAsync(
        int page,
        int pageSize,
        int? teacherId,
        int? subjectId,
        string? className,
        string? section,
        bool? activeOnly,
        CancellationToken cancellationToken = default);

    Task<TeacherTeachingPlanDto?> GetTeachingPlanAsync(
        int teacherId,
        DateOnly? fromDate,
        DateOnly? toDate,
        bool? activeOnly,
        CancellationToken cancellationToken = default);

    Task<TeacherAllocationDto> CreateAllocationAsync(CreateTeacherAllocationDto dto, CancellationToken cancellationToken = default);
    Task<TeacherAllocationDto?> UpdateAllocationAsync(int id, UpdateTeacherAllocationDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAllocationAsync(int id, CancellationToken cancellationToken = default);
}
