using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Timetables.Dtos;

namespace SchoolManagement.Application.Timetables;

public interface ITimetableService
{
    Task<PagedResult<TimetableEntryDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? className,
        string? section,
        DayOfWeek? dayOfWeek,
        int? teacherId,
        bool? activeOnly,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TimetableEntryDto>> GetByClassSectionAsync(
        string className,
        string section,
        bool? activeOnly,
        CancellationToken cancellationToken = default);

    Task<TimetableEntryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TimetableEntryDto> CreateAsync(CreateTimetableEntryDto dto, CancellationToken cancellationToken = default);
    Task<TimetableEntryDto?> UpdateAsync(int id, UpdateTimetableEntryDto dto, CancellationToken cancellationToken = default);
    Task<int> BulkUpsertWeeklyAsync(BulkUpsertWeeklyTimetableDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
