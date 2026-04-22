using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.Infrastructure.Repositories;

public class AttendanceRepository : IAttendanceRepository
{
    private readonly ApplicationDbContext _db;

    public AttendanceRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<AttendanceRecord>> GetPagedAsync(
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
        var query = _db.AttendanceRecords.AsNoTracking().Include(a => a.Student).AsQueryable();

        if (studentId.HasValue)
            query = query.Where(a => a.StudentId == studentId.Value);

        if (!string.IsNullOrWhiteSpace(className))
        {
            var c = className.Trim().ToLower();
            query = query.Where(a => a.Class.ToLower() == c);
        }

        if (!string.IsNullOrWhiteSpace(section))
        {
            var sec = section.Trim().ToLower();
            query = query.Where(a => a.Section.ToLower() == sec);
        }

        if (dateFrom.HasValue)
            query = query.Where(a => a.Date >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(a => a.Date <= dateTo.Value);

        var isAsc = !string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy?.ToLowerInvariant() switch
        {
            "date" => isAsc ? query.OrderBy(a => a.Date) : query.OrderByDescending(a => a.Date),
            "studentname" => isAsc
                ? query.OrderBy(a => a.Student!.Name)
                : query.OrderByDescending(a => a.Student!.Name),
            "class" => isAsc ? query.OrderBy(a => a.Class) : query.OrderByDescending(a => a.Class),
            "section" => isAsc ? query.OrderBy(a => a.Section) : query.OrderByDescending(a => a.Section),
            _ => isAsc ? query.OrderBy(a => a.Id) : query.OrderByDescending(a => a.Id)
        };

        var total = await query.CountAsync(cancellationToken);

        var data = await query
            .Skip(Math.Max(0, page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AttendanceRecord>
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Data = data
        };
    }

    public Task<AttendanceRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.AttendanceRecords
            .Include(a => a.Student)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public Task<AttendanceRecord?> GetByStudentAndDateAsync(int studentId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return _db.AttendanceRecords
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.StudentId == studentId && a.Date == date, cancellationToken);
    }

    public Task<bool> ExistsForStudentAndDateAsync(int studentId, DateOnly date, CancellationToken cancellationToken = default)
    {
        return _db.AttendanceRecords.AnyAsync(a => a.StudentId == studentId && a.Date == date, cancellationToken);
    }

    public async Task AddAsync(AttendanceRecord entity, CancellationToken cancellationToken = default)
    {
        await _db.AttendanceRecords.AddAsync(entity, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(AttendanceRecord entity, CancellationToken cancellationToken = default)
    {
        _db.AttendanceRecords.Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.AttendanceRecords.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        if (entity == null)
            return false;

        _db.AttendanceRecords.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task UpsertDayEntriesAsync(
        DateOnly date,
        string className,
        string section,
        IReadOnlyList<(int StudentId, AttendanceStatus Status, string? Notes)> lines,
        int? markedByUserId,
        CancellationToken cancellationToken = default)
    {
        await using var tx = await _db.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            foreach (var (studentId, status, notes) in lines)
            {
                var existing = await _db.AttendanceRecords
                    .FirstOrDefaultAsync(a => a.StudentId == studentId && a.Date == date, cancellationToken);

                if (existing == null)
                {
                    await _db.AttendanceRecords.AddAsync(
                        new AttendanceRecord
                        {
                            StudentId = studentId,
                            Date = date,
                            Status = status,
                            Class = className,
                            Section = section,
                            Notes = notes,
                            MarkedByUserId = markedByUserId,
                            CreatedAtUtc = DateTime.UtcNow
                        },
                        cancellationToken);
                }
                else
                {
                    existing.Status = status;
                    existing.Notes = notes;
                    existing.Class = className;
                    existing.Section = section;
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

    public async Task<AttendanceSummaryDto> GetSummaryAsync(
        DateOnly from,
        DateOnly to,
        string? className,
        string? section,
        CancellationToken cancellationToken = default)
    {
        var query = _db.AttendanceRecords.AsNoTracking().Where(a => a.Date >= from && a.Date <= to);

        if (!string.IsNullOrWhiteSpace(className))
        {
            var c = className.Trim().ToLower();
            query = query.Where(a => a.Class.ToLower() == c);
        }

        if (!string.IsNullOrWhiteSpace(section))
        {
            var sec = section.Trim().ToLower();
            query = query.Where(a => a.Section.ToLower() == sec);
        }

        var rows = await query.Select(a => a.Status).ToListAsync(cancellationToken);

        var summary = new AttendanceSummaryDto
        {
            From = from,
            To = to,
            TotalMarks = rows.Count,
            Present = rows.Count(s => s == AttendanceStatus.Present),
            Absent = rows.Count(s => s == AttendanceStatus.Absent),
            Late = rows.Count(s => s == AttendanceStatus.Late),
            Excused = rows.Count(s => s == AttendanceStatus.Excused),
            HalfDay = rows.Count(s => s == AttendanceStatus.HalfDay)
        };

        summary.AttendanceRatePercent = await GetAttendanceRatePercentAsync(from, to, className, section, cancellationToken);
        return summary;
    }

    public async Task<decimal?> GetAttendanceRatePercentAsync(
        DateOnly from,
        DateOnly to,
        string? className,
        string? section,
        CancellationToken cancellationToken = default)
    {
        var query = _db.AttendanceRecords.AsNoTracking()
            .Where(a => a.Date >= from && a.Date <= to && a.Status != AttendanceStatus.Excused);

        if (!string.IsNullOrWhiteSpace(className))
        {
            var c = className.Trim().ToLower();
            query = query.Where(a => a.Class.ToLower() == c);
        }

        if (!string.IsNullOrWhiteSpace(section))
        {
            var sec = section.Trim().ToLower();
            query = query.Where(a => a.Section.ToLower() == sec);
        }

        var statuses = await query.Select(a => a.Status).ToListAsync(cancellationToken);
        if (statuses.Count == 0)
            return null;

        decimal weighted = 0;
        foreach (var s in statuses)
        {
            weighted += s switch
            {
                AttendanceStatus.Present => 1m,
                AttendanceStatus.Late => 1m,
                AttendanceStatus.HalfDay => 0.5m,
                _ => 0m
            };
        }

        return Math.Round(100m * weighted / statuses.Count, 2, MidpointRounding.AwayFromZero);
    }
}
