using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.Infrastructure.Repositories;

public class TimetableRepository : ITimetableRepository
{
    private readonly ApplicationDbContext _db;

    public TimetableRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<TimetableEntry>> GetPagedAsync(
        int page,
        int pageSize,
        string? className,
        string? section,
        DayOfWeek? dayOfWeek,
        int? teacherId,
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var query = _db.TimetableEntries.AsNoTracking()
            .Include(x => x.Subject)
            .Include(x => x.Teacher)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(className))
            query = query.Where(x => x.Class.ToLower() == className.Trim().ToLower());
        if (!string.IsNullOrWhiteSpace(section))
            query = query.Where(x => x.Section.ToLower() == section.Trim().ToLower());
        if (dayOfWeek.HasValue)
            query = query.Where(x => x.DayOfWeek == dayOfWeek.Value);
        if (teacherId.HasValue)
            query = query.Where(x => x.TeacherId == teacherId.Value);
        if (activeOnly == true)
            query = query.Where(x => x.IsActive);

        var total = await query.CountAsync(cancellationToken);
        var data = await query
            .OrderBy(x => x.Class)
            .ThenBy(x => x.Section)
            .ThenBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)
            .Skip(Math.Max(0, page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<TimetableEntry> { Total = total, Page = page, PageSize = pageSize, Data = data };
    }

    public async Task<IReadOnlyList<TimetableEntry>> GetByClassSectionAsync(
        string className,
        string section,
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var query = _db.TimetableEntries.AsNoTracking()
            .Include(x => x.Subject)
            .Include(x => x.Teacher)
            .Where(x => x.Class.ToLower() == className.ToLower() && x.Section.ToLower() == section.ToLower());

        if (activeOnly == true)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.DayOfWeek)
            .ThenBy(x => x.StartTime)
            .ToListAsync(cancellationToken);
    }

    public Task<TimetableEntry?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.TimetableEntries
            .Include(x => x.Subject)
            .Include(x => x.Teacher)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<bool> ExistsClassSlotConflictAsync(string className, string section, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? exceptId, CancellationToken cancellationToken = default)
    {
        var query = _db.TimetableEntries.AsNoTracking().Where(
            x => x.Class.ToLower() == className.ToLower()
                 && x.Section.ToLower() == section.ToLower()
                 && x.DayOfWeek == dayOfWeek
                 && x.StartTime < endTime
                 && startTime < x.EndTime);
        if (exceptId.HasValue) query = query.Where(x => x.Id != exceptId.Value);
        return query.AnyAsync(cancellationToken);
    }

    public Task<bool> ExistsTeacherSlotConflictAsync(int teacherId, DayOfWeek dayOfWeek, TimeOnly startTime, TimeOnly endTime, int? exceptId, CancellationToken cancellationToken = default)
    {
        var query = _db.TimetableEntries.AsNoTracking().Where(
            x => x.TeacherId == teacherId
                 && x.DayOfWeek == dayOfWeek
                 && x.StartTime < endTime
                 && startTime < x.EndTime);
        if (exceptId.HasValue) query = query.Where(x => x.Id != exceptId.Value);
        return query.AnyAsync(cancellationToken);
    }

    public async Task UpsertWeeklyAsync(string className, string section, IReadOnlyList<TimetableEntry> entries, CancellationToken cancellationToken = default)
    {
        var existing = await _db.TimetableEntries
            .Where(x => x.Class.ToLower() == className.ToLower() && x.Section.ToLower() == section.ToLower())
            .ToListAsync(cancellationToken);

        var existingBySlot = existing.ToDictionary(x => (x.DayOfWeek, x.StartTime, x.EndTime));
        foreach (var entry in entries)
        {
            if (existingBySlot.TryGetValue((entry.DayOfWeek, entry.StartTime, entry.EndTime), out var row))
            {
                row.SubjectId = entry.SubjectId;
                row.TeacherId = entry.TeacherId;
                row.IsActive = entry.IsActive;
            }
            else
            {
                await _db.TimetableEntries.AddAsync(entry, cancellationToken);
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAsync(TimetableEntry row, CancellationToken cancellationToken = default)
    {
        await _db.TimetableEntries.AddAsync(row, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TimetableEntry row, CancellationToken cancellationToken = default)
    {
        _db.TimetableEntries.Update(row);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _db.TimetableEntries.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row == null) return false;
        _db.TimetableEntries.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
