using AutoMapper;
using SchoolManagement.Application.Attendance.Dtos;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;

namespace SchoolManagement.Application.Attendance;

public class AttendanceService : IAttendanceService
{
    private readonly IAttendanceRepository _attendance;
    private readonly IStudentRepository _students;
    private readonly IMapper _mapper;

    public AttendanceService(IAttendanceRepository attendance, IStudentRepository students, IMapper mapper)
    {
        _attendance = attendance;
        _students = students;
        _mapper = mapper;
    }

    public async Task<PagedResult<AttendanceRecordDto>> GetPagedAsync(
        int page,
        int pageSize,
        int? studentId,
        string? className,
        string? section,
        DateOnly? dateFrom,
        DateOnly? dateTo,
        string sortBy,
        string order,
        CancellationToken cancellationToken = default)
    {
        var pageResult = await _attendance.GetPagedAsync(page, pageSize, studentId, className, section, dateFrom, dateTo, sortBy, order, cancellationToken);
        return new PagedResult<AttendanceRecordDto>
        {
            Total = pageResult.Total,
            Page = pageResult.Page,
            PageSize = pageResult.PageSize,
            Data = _mapper.Map<IReadOnlyList<AttendanceRecordDto>>(pageResult.Data)
        };
    }

    public async Task<AttendanceRecordDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _attendance.GetByIdAsync(id, cancellationToken);
        return entity == null ? null : _mapper.Map<AttendanceRecordDto>(entity);
    }

    public async Task<AttendanceRecordDto> CreateAsync(CreateAttendanceRecordDto dto, int? markedByUserId, CancellationToken cancellationToken = default)
    {
        var student = await _students.GetByIdAsync(dto.StudentId, cancellationToken);
        if (student == null)
            throw new InvalidOperationException("Student not found.");

        if (await _attendance.ExistsForStudentAndDateAsync(dto.StudentId, dto.Date, cancellationToken))
            throw new InvalidOperationException("Attendance for this student on this date already exists. Use bulk mark or update instead.");

        var entity = new AttendanceRecord
        {
            StudentId = dto.StudentId,
            Date = dto.Date,
            Status = dto.Status,
            Class = dto.Class.Trim(),
            Section = dto.Section.Trim(),
            Notes = dto.Notes,
            MarkedByUserId = markedByUserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _attendance.AddAsync(entity, cancellationToken);
        var created = await _attendance.GetByIdAsync(entity.Id, cancellationToken) ?? entity;
        return _mapper.Map<AttendanceRecordDto>(created);
    }

    public async Task<AttendanceRecordDto?> UpdateAsync(int id, UpdateAttendanceRecordDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _attendance.GetByIdAsync(id, cancellationToken);
        if (entity == null)
            return null;

        entity.Status = dto.Status;
        entity.Notes = dto.Notes;
        await _attendance.UpdateAsync(entity, cancellationToken);
        var updated = await _attendance.GetByIdAsync(id, cancellationToken);
        return updated == null ? null : _mapper.Map<AttendanceRecordDto>(updated);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return _attendance.DeleteAsync(id, cancellationToken);
    }

    public async Task<int> BulkMarkDayAsync(BulkMarkAttendanceDayDto dto, int? markedByUserId, CancellationToken cancellationToken = default)
    {
        if (dto.Lines.Count == 0)
            return 0;

        var ids = dto.Lines.Select(l => l.StudentId).Distinct().ToList();
        var students = await _students.ListByIdsAsync(ids, cancellationToken);
        var byId = students.ToDictionary(s => s.Id);

        foreach (var line in dto.Lines)
        {
            if (!byId.ContainsKey(line.StudentId))
                throw new InvalidOperationException($"Student {line.StudentId} was not found.");
        }

        await _attendance.UpsertDayEntriesAsync(dto.Date, dto.Class.Trim(), dto.Section.Trim(), dto.Lines.Select(l => (l.StudentId, l.Status, l.Notes)).ToList(), markedByUserId, cancellationToken);
        return dto.Lines.Count;
    }

    public Task<AttendanceSummaryDto> GetSummaryAsync(DateOnly from, DateOnly to, string? className, string? section, CancellationToken cancellationToken = default)
    {
        return _attendance.GetSummaryAsync(from, to, className, section, cancellationToken);
    }
}
