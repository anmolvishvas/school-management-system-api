using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Common.Interfaces;

public interface IPeriodAttendanceRepository
{
    Task<PagedResult<PeriodAttendanceRecord>> GetPagedAsync(
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

    Task UpsertPeriodEntriesAsync(
        DateOnly date,
        int hourNumber,
        int subjectId,
        int teacherUserId,
        string className,
        string section,
        IReadOnlyList<(int StudentId, AttendanceStatus Status, string? Notes)> lines,
        int? markedByUserId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ObservedWeeklyPeriodSlot>> GetObservedWeeklySlotsAsync(
        int teacherUserId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);
}
