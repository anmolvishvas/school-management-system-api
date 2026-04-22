using SchoolManagement.Application.Attendance.Dtos;
using SchoolManagement.Application.Common.Models;

namespace SchoolManagement.Application.Attendance;

public interface IAttendanceService
{
    Task<PagedResult<AttendanceRecordDto>> GetPagedAsync(
        int page,
        int pageSize,
        int? studentId,
        string? className,
        string? section,
        DateOnly? dateFrom,
        DateOnly? dateTo,
        string sortBy,
        string order,
        CancellationToken cancellationToken = default);

    Task<AttendanceRecordDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<AttendanceRecordDto> CreateAsync(CreateAttendanceRecordDto dto, int? markedByUserId, CancellationToken cancellationToken = default);

    Task<AttendanceRecordDto?> UpdateAsync(int id, UpdateAttendanceRecordDto dto, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task<int> BulkMarkDayAsync(BulkMarkAttendanceDayDto dto, int? markedByUserId, CancellationToken cancellationToken = default);

    Task<AttendanceSummaryDto> GetSummaryAsync(
        DateOnly from,
        DateOnly to,
        string? className,
        string? section,
        CancellationToken cancellationToken = default);
}
