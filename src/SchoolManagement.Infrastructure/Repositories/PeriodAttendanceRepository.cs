using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.Infrastructure.Repositories;

public class PeriodAttendanceRepository : IPeriodAttendanceRepository
{
    private readonly ApplicationDbContext _db;

    public PeriodAttendanceRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<PeriodAttendanceRecord>> GetPagedAsync(int page, int pageSize, int? studentId, int? subjectId, string? className, string? section, DateOnly? dateFrom, DateOnly? dateTo, int? hourNumber, string sortBy, string order, CancellationToken cancellationToken = default)
    {
        var query = _db.PeriodAttendanceRecords.AsNoTracking().Include(p => p.Student).Include(p => p.Subject).AsQueryable();
        if (studentId.HasValue) query = query.Where(x => x.StudentId == studentId.Value);
        if (subjectId.HasValue) query = query.Where(x => x.SubjectId == subjectId.Value);
        if (!string.IsNullOrWhiteSpace(className)) query = query.Where(x => x.Class.ToLower() == className.Trim().ToLower());
        if (!string.IsNullOrWhiteSpace(section)) query = query.Where(x => x.Section.ToLower() == section.Trim().ToLower());
        if (dateFrom.HasValue) query = query.Where(x => x.Date >= dateFrom.Value);
        if (dateTo.HasValue) query = query.Where(x => x.Date <= dateTo.Value);
        if (hourNumber.HasValue) query = query.Where(x => x.HourNumber == hourNumber.Value);

        var isAsc = !string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);
        query = sortBy?.ToLowerInvariant() switch
        {
            "hour" => isAsc ? query.OrderBy(x => x.HourNumber) : query.OrderByDescending(x => x.HourNumber),
            "studentname" => isAsc ? query.OrderBy(x => x.Student!.Name) : query.OrderByDescending(x => x.Student!.Name),
            _ => isAsc ? query.OrderBy(x => x.Date).ThenBy(x => x.HourNumber) : query.OrderByDescending(x => x.Date).ThenByDescending(x => x.HourNumber)
        };

        var total = await query.CountAsync(cancellationToken);
        var data = await query.Skip(Math.Max(0, page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<PeriodAttendanceRecord> { Total = total, Page = page, PageSize = pageSize, Data = data };
    }

    public async Task UpsertPeriodEntriesAsync(DateOnly date, int hourNumber, int subjectId, int teacherUserId, string className, string section, IReadOnlyList<(int StudentId, AttendanceStatus Status, string? Notes)> lines, int? markedByUserId, CancellationToken cancellationToken = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var (studentId, status, notes) in lines)
            {
                var existing = await _db.PeriodAttendanceRecords.FirstOrDefaultAsync(x => x.StudentId == studentId && x.Date == date && x.HourNumber == hourNumber && x.SubjectId == subjectId, cancellationToken);
                if (existing == null)
                {
                    await _db.PeriodAttendanceRecords.AddAsync(new PeriodAttendanceRecord
                    {
                        StudentId = studentId,
                        SubjectId = subjectId,
                        TeacherUserId = teacherUserId,
                        Date = date,
                        HourNumber = hourNumber,
                        Status = status,
                        Class = className,
                        Section = section,
                        Notes = notes,
                        MarkedByUserId = markedByUserId,
                        CreatedAtUtc = DateTime.UtcNow
                    }, cancellationToken);
                }
                else
                {
                    existing.Status = status;
                    existing.Notes = notes;
                    existing.Class = className;
                    existing.Section = section;
                    existing.TeacherUserId = teacherUserId;
                    existing.MarkedByUserId = markedByUserId;
                }
            }

            await _db.SaveChangesAsync(cancellationToken);
            await tx.CommitAsync(cancellationToken);
        }
        catch
        {
            await tx.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IReadOnlyList<ObservedWeeklyPeriodSlot>> GetObservedWeeklySlotsAsync(
        int teacherUserId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
    {
        return await _db.PeriodAttendanceRecords
            .AsNoTracking()
            .Where(x => x.TeacherUserId == teacherUserId && x.Date >= from && x.Date <= to)
            .Select(x => new ObservedWeeklyPeriodSlot
            {
                SubjectId = x.SubjectId,
                Class = x.Class,
                Section = x.Section,
                DayOfWeek = x.Date.DayOfWeek,
                HourNumber = x.HourNumber
            })
            .Distinct()
            .OrderBy(x => x.Class)
            .ThenBy(x => x.Section)
            .ThenBy(x => x.SubjectId)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => x.HourNumber)
            .ToListAsync(cancellationToken);
    }
}
