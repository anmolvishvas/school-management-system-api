using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Common.Interfaces;

public interface ITimetableRepository
{
    Task<PagedResult<TimetableEntry>> GetPagedAsync(
        int page,
        int pageSize,
        string? className,
        string? section,
        DayOfWeek? dayOfWeek,
        int? teacherId,
        bool? activeOnly,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TimetableEntry>> GetByClassSectionAsync(
        string className,
        string section,
        bool? activeOnly,
        CancellationToken cancellationToken = default);

    Task<TimetableEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> ExistsClassSlotConflictAsync(string className, string section, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? exceptId, CancellationToken cancellationToken = default);
    Task<bool> ExistsTeacherSlotConflictAsync(int teacherId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? exceptId, CancellationToken cancellationToken = default);
    Task UpsertWeeklyAsync(string className, string section, IReadOnlyList<TimetableEntry> entries, CancellationToken cancellationToken = default);
    Task AddAsync(TimetableEntry row, CancellationToken cancellationToken = default);
    Task UpdateAsync(TimetableEntry row, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
