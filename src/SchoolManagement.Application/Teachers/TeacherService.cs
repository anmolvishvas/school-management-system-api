using AutoMapper;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Teachers.Dtos;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Teachers;

public class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _teachers;
    private readonly ITeacherAllocationRepository _allocations;
    private readonly IUserRepository _users;
    private readonly ISubjectRepository _subjects;
    private readonly IPeriodAttendanceRepository _periodAttendance;
    private readonly IMapper _mapper;

    public TeacherService(
        ITeacherRepository teachers,
        ITeacherAllocationRepository allocations,
        IUserRepository users,
        ISubjectRepository subjects,
        IPeriodAttendanceRepository periodAttendance,
        IMapper mapper)
    {
        _teachers = teachers;
        _allocations = allocations;
        _users = users;
        _subjects = subjects;
        _periodAttendance = periodAttendance;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<TeacherDto>> GetTeachersAsync(bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var rows = await _teachers.GetAllAsync(activeOnly, cancellationToken);
        return _mapper.Map<IReadOnlyList<TeacherDto>>(rows);
    }

    public async Task<TeacherDto?> GetTeacherByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _teachers.GetByIdAsync(id, cancellationToken);
        return row == null ? null : _mapper.Map<TeacherDto>(row);
    }

    public async Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto dto, CancellationToken cancellationToken = default)
    {
        var user = await _users.GetByIdAsync(dto.UserId, cancellationToken) ?? throw new InvalidOperationException("User not found.");
        if (!string.Equals(user.Role, ApplicationRoles.Teacher, StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(user.Role, ApplicationRoles.Admin, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("User role must be Teacher (or Admin).");
        }

        var existing = await _teachers.GetByUserIdAsync(dto.UserId, cancellationToken);
        if (existing != null) throw new InvalidOperationException("Teacher profile already exists for this user.");

        var teacher = new Teacher
        {
            UserId = dto.UserId,
            FullName = dto.FullName.Trim(),
            Phone = dto.Phone,
            Email = dto.Email,
            IsActive = dto.IsActive
        };

        await _teachers.AddAsync(teacher, cancellationToken);
        var created = await _teachers.GetByIdAsync(teacher.Id, cancellationToken) ?? teacher;
        return _mapper.Map<TeacherDto>(created);
    }

    public async Task<TeacherDto?> UpdateTeacherAsync(int id, UpdateTeacherDto dto, CancellationToken cancellationToken = default)
    {
        var teacher = await _teachers.GetByIdAsync(id, cancellationToken);
        if (teacher == null) return null;

        teacher.FullName = dto.FullName.Trim();
        teacher.Phone = dto.Phone;
        teacher.Email = dto.Email;
        teacher.IsActive = dto.IsActive;

        await _teachers.UpdateAsync(teacher, cancellationToken);
        var updated = await _teachers.GetByIdAsync(id, cancellationToken) ?? teacher;
        return _mapper.Map<TeacherDto>(updated);
    }

    public Task<bool> DeleteTeacherAsync(int id, CancellationToken cancellationToken = default)
        => _teachers.DeleteAsync(id, cancellationToken);

    public async Task<PagedResult<TeacherAllocationDto>> GetAllocationsAsync(int page, int pageSize, int? teacherId, int? subjectId, string? className, string? section, bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var rows = await _allocations.GetPagedAsync(page, pageSize, teacherId, subjectId, className, section, activeOnly, cancellationToken);
        return new PagedResult<TeacherAllocationDto>
        {
            Total = rows.Total,
            Page = rows.Page,
            PageSize = rows.PageSize,
            Data = _mapper.Map<IReadOnlyList<TeacherAllocationDto>>(rows.Data)
        };
    }

    public async Task<TeacherTeachingPlanDto?> GetTeachingPlanAsync(
        int teacherId,
        DateOnly? fromDate,
        DateOnly? toDate,
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var teacher = await _teachers.GetByIdAsync(teacherId, cancellationToken);
        if (teacher == null)
            return null;

        var to = toDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var from = fromDate ?? to.AddDays(-90);
        if (to < from)
            throw new InvalidOperationException("'toDate' must be on or after 'fromDate'.");

        var allocations = await _allocations.ListByTeacherAsync(teacherId, activeOnly, cancellationToken);
        var observed = await _periodAttendance.GetObservedWeeklySlotsAsync(teacher.UserId, from, to, cancellationToken);

        var observedLookup = observed
            .GroupBy(x => $"{x.Class}|{x.Section}|{x.SubjectId}")
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyList<WeeklyPeriodSlotDto>)g
                    .Select(x => new WeeklyPeriodSlotDto { DayOfWeek = x.DayOfWeek, HourNumber = x.HourNumber })
                    .DistinctBy(x => new { x.DayOfWeek, x.HourNumber })
                    .OrderBy(x => x.DayOfWeek)
                    .ThenBy(x => x.HourNumber)
                    .ToList());

        var classSections = allocations
            .GroupBy(x => new { x.Class, x.Section })
            .OrderBy(g => g.Key.Class)
            .ThenBy(g => g.Key.Section)
            .Select(g =>
            {
                var subjects = g
                    .OrderBy(x => x.Subject!.Name)
                    .Select(a =>
                    {
                        var key = $"{a.Class}|{a.Section}|{a.SubjectId}";
                        observedLookup.TryGetValue(key, out var slots);
                        return new TeachingPlanSubjectDto
                        {
                            AllocationId = a.Id,
                            SubjectId = a.SubjectId,
                            SubjectName = a.Subject?.Name ?? string.Empty,
                            IsClassTeacher = a.IsClassTeacher,
                            IsActive = a.IsActive,
                            WeeklyPeriods = slots ?? Array.Empty<WeeklyPeriodSlotDto>()
                        };
                    })
                    .ToList();

                return new TeachingPlanClassSectionDto
                {
                    Class = g.Key.Class,
                    Section = g.Key.Section,
                    Subjects = subjects
                };
            })
            .ToList();

        return new TeacherTeachingPlanDto
        {
            TeacherId = teacher.Id,
            UserId = teacher.UserId,
            TeacherName = teacher.FullName,
            Username = teacher.User?.Username ?? string.Empty,
            FromDate = from,
            ToDate = to,
            ClassSections = classSections
        };
    }

    public async Task<TeacherAllocationDto> CreateAllocationAsync(CreateTeacherAllocationDto dto, CancellationToken cancellationToken = default)
    {
        var teacher = await _teachers.GetByIdAsync(dto.TeacherId, cancellationToken) ?? throw new InvalidOperationException("Teacher not found.");
        _ = await _subjects.GetByIdAsync(dto.SubjectId, cancellationToken) ?? throw new InvalidOperationException("Subject not found.");

        if (await _allocations.ExistsDuplicateAsync(dto.TeacherId, dto.SubjectId, dto.Class.Trim(), dto.Section.Trim(), null, cancellationToken))
            throw new InvalidOperationException("Duplicate allocation exists for teacher+subject+class+section.");

        var row = new TeacherClassSubjectAllocation
        {
            TeacherId = dto.TeacherId,
            SubjectId = dto.SubjectId,
            Class = dto.Class.Trim(),
            Section = dto.Section.Trim(),
            IsClassTeacher = dto.IsClassTeacher,
            IsActive = dto.IsActive
        };

        await _allocations.AddAsync(row, cancellationToken);
        var created = await _allocations.GetByIdAsync(row.Id, cancellationToken) ?? row;
        return _mapper.Map<TeacherAllocationDto>(created);
    }

    public async Task<TeacherAllocationDto?> UpdateAllocationAsync(int id, UpdateTeacherAllocationDto dto, CancellationToken cancellationToken = default)
    {
        var row = await _allocations.GetByIdAsync(id, cancellationToken);
        if (row == null) return null;

        _ = await _subjects.GetByIdAsync(dto.SubjectId, cancellationToken) ?? throw new InvalidOperationException("Subject not found.");
        _ = await _teachers.GetByIdAsync(row.TeacherId, cancellationToken) ?? throw new InvalidOperationException("Teacher not found.");

        if (await _allocations.ExistsDuplicateAsync(row.TeacherId, dto.SubjectId, dto.Class.Trim(), dto.Section.Trim(), id, cancellationToken))
            throw new InvalidOperationException("Duplicate allocation exists for teacher+subject+class+section.");

        row.SubjectId = dto.SubjectId;
        row.Class = dto.Class.Trim();
        row.Section = dto.Section.Trim();
        row.IsClassTeacher = dto.IsClassTeacher;
        row.IsActive = dto.IsActive;

        await _allocations.UpdateAsync(row, cancellationToken);
        var updated = await _allocations.GetByIdAsync(id, cancellationToken) ?? row;
        return _mapper.Map<TeacherAllocationDto>(updated);
    }

    public Task<bool> DeleteAllocationAsync(int id, CancellationToken cancellationToken = default)
        => _allocations.DeleteAsync(id, cancellationToken);
}
