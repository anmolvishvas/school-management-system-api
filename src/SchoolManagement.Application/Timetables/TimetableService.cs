using AutoMapper;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Timetables.Dtos;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Timetables;

public class TimetableService : ITimetableService
{
    private readonly ITimetableRepository _timetable;
    private readonly ITeacherRepository _teachers;
    private readonly ISubjectRepository _subjects;
    private readonly IMapper _mapper;

    public TimetableService(
        ITimetableRepository timetable,
        ITeacherRepository teachers,
        ISubjectRepository subjects,
        IMapper mapper)
    {
        _timetable = timetable;
        _teachers = teachers;
        _subjects = subjects;
        _mapper = mapper;
    }

    public async Task<PagedResult<TimetableEntryDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? className,
        string? section,
        DayOfWeek? dayOfWeek,
        int? teacherId,
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var rows = await _timetable.GetPagedAsync(page, pageSize, className, section, dayOfWeek, teacherId, activeOnly, cancellationToken);
        return new PagedResult<TimetableEntryDto>
        {
            Total = rows.Total,
            Page = rows.Page,
            PageSize = rows.PageSize,
            Data = _mapper.Map<IReadOnlyList<TimetableEntryDto>>(rows.Data)
        };
    }

    public async Task<IReadOnlyList<TimetableEntryDto>> GetByClassSectionAsync(
        string className,
        string section,
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var rows = await _timetable.GetByClassSectionAsync(className.Trim(), section.Trim(), activeOnly, cancellationToken);
        return _mapper.Map<IReadOnlyList<TimetableEntryDto>>(rows);
    }

    public async Task<TimetableEntryDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _timetable.GetByIdAsync(id, cancellationToken);
        return row == null ? null : _mapper.Map<TimetableEntryDto>(row);
    }

    public async Task<TimetableEntryDto> CreateAsync(CreateTimetableEntryDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedClass = dto.Class.Trim();
        var normalizedSection = dto.Section.Trim();

        await EnsureValidReferencesAsync(normalizedClass, normalizedSection, dto.SubjectId, dto.TeacherId, cancellationToken);

        if (await _timetable.ExistsClassSlotConflictAsync(normalizedClass, normalizedSection, dto.DayOfWeek, dto.StartTime, dto.EndTime, null, cancellationToken))
            throw new InvalidOperationException("Class/section already has a timetable entry for this day and period.");

        if (await _timetable.ExistsTeacherSlotConflictAsync(dto.TeacherId, dto.DayOfWeek, dto.StartTime, dto.EndTime, null, cancellationToken))
            throw new InvalidOperationException("Teacher already has a timetable entry for this day and period.");

        var row = new TimetableEntry
        {
            Class = normalizedClass,
            Section = normalizedSection,
            DayOfWeek = dto.DayOfWeek,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            SubjectId = dto.SubjectId,
            TeacherId = dto.TeacherId,
            IsActive = dto.IsActive
        };

        await _timetable.AddAsync(row, cancellationToken);
        var created = await _timetable.GetByIdAsync(row.Id, cancellationToken) ?? row;
        return _mapper.Map<TimetableEntryDto>(created);
    }

    public async Task<TimetableEntryDto?> UpdateAsync(int id, UpdateTimetableEntryDto dto, CancellationToken cancellationToken = default)
    {
        var row = await _timetable.GetByIdAsync(id, cancellationToken);
        if (row == null)
            return null;

        var normalizedClass = dto.Class.Trim();
        var normalizedSection = dto.Section.Trim();

        await EnsureValidReferencesAsync(normalizedClass, normalizedSection, dto.SubjectId, dto.TeacherId, cancellationToken);

        if (await _timetable.ExistsClassSlotConflictAsync(normalizedClass, normalizedSection, dto.DayOfWeek, dto.StartTime, dto.EndTime, id, cancellationToken))
            throw new InvalidOperationException("Class/section already has a timetable entry for this day and period.");

        if (await _timetable.ExistsTeacherSlotConflictAsync(dto.TeacherId, dto.DayOfWeek, dto.StartTime, dto.EndTime, id, cancellationToken))
            throw new InvalidOperationException("Teacher already has a timetable entry for this day and period.");

        row.Class = normalizedClass;
        row.Section = normalizedSection;
        row.DayOfWeek = dto.DayOfWeek;
        row.StartTime = dto.StartTime;
        row.EndTime = dto.EndTime;
        row.SubjectId = dto.SubjectId;
        row.TeacherId = dto.TeacherId;
        row.IsActive = dto.IsActive;

        await _timetable.UpdateAsync(row, cancellationToken);
        var updated = await _timetable.GetByIdAsync(id, cancellationToken) ?? row;
        return _mapper.Map<TimetableEntryDto>(updated);
    }

    public async Task<int> BulkUpsertWeeklyAsync(BulkUpsertWeeklyTimetableDto dto, CancellationToken cancellationToken = default)
    {
        var normalizedClass = dto.Class.Trim();
        var normalizedSection = dto.Section.Trim();

        if (dto.Lines.Count == 0)
            return 0;

        var duplicateClassSlots = dto.Lines
            .GroupBy(x => new { x.DayOfWeek, x.StartTime, x.EndTime })
            .Where(g => g.Count() > 1)
            .Select(g => $"{g.Key.DayOfWeek} {g.Key.StartTime:HH\\:mm}-{g.Key.EndTime:HH\\:mm}")
            .ToList();
        if (duplicateClassSlots.Count > 0)
            throw new InvalidOperationException($"Duplicate class slots in request: {string.Join(", ", duplicateClassSlots)}");

        var duplicateTeacherSlots = dto.Lines
            .GroupBy(x => new { x.TeacherId, x.DayOfWeek, x.StartTime, x.EndTime })
            .Where(g => g.Count() > 1)
            .Select(g => $"T{g.Key.TeacherId}-{g.Key.DayOfWeek} {g.Key.StartTime:HH\\:mm}-{g.Key.EndTime:HH\\:mm}")
            .ToList();
        if (duplicateTeacherSlots.Count > 0)
            throw new InvalidOperationException($"Duplicate teacher slots in request: {string.Join(", ", duplicateTeacherSlots)}");

        var existing = await _timetable.GetByClassSectionAsync(normalizedClass, normalizedSection, null, cancellationToken);
        var existingBySlot = existing.ToDictionary(x => (x.DayOfWeek, x.StartTime, x.EndTime));

        foreach (var line in dto.Lines)
        {
            await EnsureValidReferencesAsync(normalizedClass, normalizedSection, line.SubjectId, line.TeacherId, cancellationToken);

            var existingEntryId = existingBySlot.TryGetValue((line.DayOfWeek, line.StartTime, line.EndTime), out var existingRow)
                ? existingRow.Id
                : (int?)null;

            if (await _timetable.ExistsTeacherSlotConflictAsync(line.TeacherId, line.DayOfWeek, line.StartTime, line.EndTime, existingEntryId, cancellationToken))
                throw new InvalidOperationException($"Teacher {line.TeacherId} already has a timetable entry for {line.DayOfWeek}, {line.StartTime:HH\\:mm}-{line.EndTime:HH\\:mm}.");
        }

        var rows = dto.Lines.Select(line => new TimetableEntry
        {
            Class = normalizedClass,
            Section = normalizedSection,
            DayOfWeek = line.DayOfWeek,
            StartTime = line.StartTime,
            EndTime = line.EndTime,
            SubjectId = line.SubjectId,
            TeacherId = line.TeacherId,
            IsActive = line.IsActive
        }).ToList();

        await _timetable.UpsertWeeklyAsync(normalizedClass, normalizedSection, rows, cancellationToken);
        return rows.Count;
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
        => _timetable.DeleteAsync(id, cancellationToken);

    private async Task EnsureValidReferencesAsync(string className, string section, int subjectId, int teacherId, CancellationToken cancellationToken)
    {
        var teacher = await _teachers.GetByIdAsync(teacherId, cancellationToken)
            ?? throw new InvalidOperationException("Teacher not found.");
        _ = await _subjects.GetByIdAsync(subjectId, cancellationToken)
            ?? throw new InvalidOperationException("Subject not found.");

        if (!teacher.IsActive)
            throw new InvalidOperationException("Teacher is inactive.");
    }
}
