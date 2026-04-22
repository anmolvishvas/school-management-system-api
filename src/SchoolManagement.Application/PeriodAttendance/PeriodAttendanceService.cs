using AutoMapper;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.PeriodAttendance.Dtos;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.PeriodAttendance;

public class PeriodAttendanceService : IPeriodAttendanceService
{
    private readonly ISubjectRepository _subjects;
    private readonly ITeacherAllocationRepository _allocations;
    private readonly IStudentRepository _students;
    private readonly IPeriodAttendanceRepository _periodAttendance;
    private readonly IMapper _mapper;

    public PeriodAttendanceService(
        ISubjectRepository subjects,
        ITeacherAllocationRepository allocations,
        IStudentRepository students,
        IPeriodAttendanceRepository periodAttendance,
        IMapper mapper)
    {
        _subjects = subjects;
        _allocations = allocations;
        _students = students;
        _periodAttendance = periodAttendance;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SubjectDto>> GetSubjectsAsync(bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var rows = await _subjects.GetAllAsync(activeOnly, cancellationToken);
        return _mapper.Map<IReadOnlyList<SubjectDto>>(rows);
    }

    public async Task<SubjectDto?> GetSubjectByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _subjects.GetByIdAsync(id, cancellationToken);
        return row == null ? null : _mapper.Map<SubjectDto>(row);
    }

    public async Task<SubjectDto> CreateSubjectAsync(CreateSubjectDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _subjects.GetByNameAsync(dto.Name.Trim(), cancellationToken);
        if (existing != null)
            throw new InvalidOperationException("Subject name already exists.");

        var subject = new Subject
        {
            Name = dto.Name.Trim(),
            IsActive = dto.IsActive
        };

        await _subjects.AddAsync(subject, cancellationToken);
        var created = await _subjects.GetByIdAsync(subject.Id, cancellationToken) ?? subject;
        return _mapper.Map<SubjectDto>(created);
    }

    public async Task<SubjectDto?> UpdateSubjectAsync(int id, UpdateSubjectDto dto, CancellationToken cancellationToken = default)
    {
        var subject = await _subjects.GetByIdAsync(id, cancellationToken);
        if (subject == null)
            return null;

        var other = await _subjects.GetByNameAsync(dto.Name.Trim(), cancellationToken);
        if (other != null && other.Id != id)
            throw new InvalidOperationException("Subject name already exists.");

        subject.Name = dto.Name.Trim();
        subject.IsActive = dto.IsActive;
        await _subjects.UpdateAsync(subject, cancellationToken);

        var updated = await _subjects.GetByIdAsync(id, cancellationToken) ?? subject;
        return _mapper.Map<SubjectDto>(updated);
    }

    public Task<bool> DeleteSubjectAsync(int id, CancellationToken cancellationToken = default)
    {
        return _subjects.DeleteAsync(id, cancellationToken);
    }

    public async Task<PagedResult<PeriodAttendanceRecordDto>> GetPeriodAttendanceAsync(
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
        CancellationToken cancellationToken = default)
    {
        var rows = await _periodAttendance.GetPagedAsync(page, pageSize, studentId, subjectId, className, section, dateFrom, dateTo, hourNumber, sortBy, order, cancellationToken);
        return new PagedResult<PeriodAttendanceRecordDto>
        {
            Total = rows.Total,
            Page = rows.Page,
            PageSize = rows.PageSize,
            Data = _mapper.Map<IReadOnlyList<PeriodAttendanceRecordDto>>(rows.Data)
        };
    }

    public async Task<int> BulkMarkPeriodAsync(BulkMarkPeriodAttendanceDto dto, int? markedByUserId, CancellationToken cancellationToken = default)
    {
        if (dto.Lines.Count == 0)
            return 0;

        _ = await _subjects.GetByIdAsync(dto.SubjectId, cancellationToken)
            ?? throw new InvalidOperationException("Subject not found.");
        var normalizedClass = dto.Class.Trim();
        var normalizedSection = dto.Section.Trim();
        var allocation = await _allocations.GetActiveByClassSectionSubjectAsync(normalizedClass, normalizedSection, dto.SubjectId, cancellationToken)
            ?? throw new InvalidOperationException("No active teacher allocation found for this class/section/subject.");

        var ids = dto.Lines.Select(l => l.StudentId).Distinct().ToList();
        var students = await _students.ListByIdsAsync(ids, cancellationToken);
        var byId = students.ToDictionary(s => s.Id);

        foreach (var line in dto.Lines)
        {
            if (!byId.TryGetValue(line.StudentId, out var student))
                throw new InvalidOperationException($"Student {line.StudentId} was not found.");

            if (!string.Equals(student.Class.Trim(), normalizedClass, StringComparison.OrdinalIgnoreCase) ||
                !string.Equals(student.Section.Trim(), normalizedSection, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Student {student.Id} is in {student.Class}/{student.Section}, not {dto.Class}/{dto.Section}.");
            }
        }

        await _periodAttendance.UpsertPeriodEntriesAsync(
            dto.Date,
            dto.HourNumber,
            dto.SubjectId,
            allocation.Teacher!.UserId,
            normalizedClass,
            normalizedSection,
            dto.Lines.Select(l => (l.StudentId, l.Status, l.Notes)).ToList(),
            markedByUserId,
            cancellationToken);

        return dto.Lines.Count;
    }
}
