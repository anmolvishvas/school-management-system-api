using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Common.Interfaces;

public interface IAttendanceRepository
{
    Task<PagedResult<AttendanceRecord>> GetPagedAsync(
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

    Task<AttendanceRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<AttendanceRecord?> GetByStudentAndDateAsync(int studentId, DateOnly date, CancellationToken cancellationToken = default);

    Task<bool> ExistsForStudentAndDateAsync(int studentId, DateOnly date, CancellationToken cancellationToken = default);

    Task AddAsync(AttendanceRecord entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(AttendanceRecord entity, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);

    Task UpsertDayEntriesAsync(
        DateOnly date,
        string className,
        string section,
        IReadOnlyList<(int StudentId, AttendanceStatus Status, string? Notes)> lines,
        int? markedByUserId,
        CancellationToken cancellationToken = default);

    Task<AttendanceSummaryDto> GetSummaryAsync(
        DateOnly from,
        DateOnly to,
        string? className,
        string? section,
        CancellationToken cancellationToken = default);

    Task<decimal?> GetAttendanceRatePercentAsync(
        DateOnly from,
        DateOnly to,
        string? className,
        string? section,
        CancellationToken cancellationToken = default);
}
