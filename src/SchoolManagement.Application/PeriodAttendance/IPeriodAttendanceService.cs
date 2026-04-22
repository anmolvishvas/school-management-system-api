using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.PeriodAttendance.Dtos;

namespace SchoolManagement.Application.PeriodAttendance;

public interface IPeriodAttendanceService
{
    Task<IReadOnlyList<SubjectDto>> GetSubjectsAsync(bool? activeOnly, CancellationToken cancellationToken = default);
    Task<SubjectDto?> GetSubjectByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto, CancellationToken cancellationToken = default);
    Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteSubjectAsync(int id, CancellationToken cancellationToken = default);

    Task<PagedResult<PeriodAttendanceRecordDto>> GetPeriodAttendanceAsync(
        int page,
        int pageSize,
        int? studentId,
        int? subjectId,
        string? className,
        string? section,
        DateOnly? dateFrom,
        DateOnly? dateTo,
        int? hourNumber,
        string sortBy,
        string order,
        CancellationToken cancellationToken = default);

    Task<int> BulkMarkPeriodAsync(BulkMarkPeriodAttendanceDto dto, int? markedByUserId, CancellationToken cancellationToken = default);
}
