using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.Infrastructure.Repositories;

public class TeacherAllocationRepository : ITeacherAllocationRepository
{
    private readonly ApplicationDbContext _db;

    public TeacherAllocationRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<TeacherClassSubjectAllocation>> GetPagedAsync(int page, int pageSize, int? teacherId, int? subjectId, string? className, string? section, bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var query = _db.TeacherClassSubjectAllocations.AsNoTracking()
            .Include(x => x.Teacher)
            .ThenInclude(t => t!.User)
            .Include(x => x.Subject)
            .AsQueryable();

        if (teacherId.HasValue) query = query.Where(x => x.TeacherId == teacherId.Value);
        if (subjectId.HasValue) query = query.Where(x => x.SubjectId == subjectId.Value);
        if (!string.IsNullOrWhiteSpace(className)) query = query.Where(x => x.Class.ToLower() == className.Trim().ToLower());
        if (!string.IsNullOrWhiteSpace(section)) query = query.Where(x => x.Section.ToLower() == section.Trim().ToLower());
        if (activeOnly == true) query = query.Where(x => x.IsActive);

        var total = await query.CountAsync(cancellationToken);
        var data = await query.OrderBy(x => x.Class).ThenBy(x => x.Section).ThenBy(x => x.Subject!.Name)
            .Skip(Math.Max(0, page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return new PagedResult<TeacherClassSubjectAllocation> { Total = total, Page = page, PageSize = pageSize, Data = data };
    }

    public Task<TeacherClassSubjectAllocation?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.TeacherClassSubjectAllocations.AsNoTracking()
            .Include(x => x.Teacher).ThenInclude(t => t!.User)
            .Include(x => x.Subject)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<TeacherClassSubjectAllocation>> ListByTeacherAsync(int teacherId, bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var query = _db.TeacherClassSubjectAllocations.AsNoTracking()
            .Include(x => x.Teacher).ThenInclude(t => t!.User)
            .Include(x => x.Subject)
            .Where(x => x.TeacherId == teacherId);

        if (activeOnly == true)
            query = query.Where(x => x.IsActive);

        return await query
            .OrderBy(x => x.Class)
            .ThenBy(x => x.Section)
            .ThenBy(x => x.Subject!.Name)
            .ToListAsync(cancellationToken);
    }

    public Task<TeacherClassSubjectAllocation?> GetActiveByClassSectionSubjectAsync(string className, string section, int subjectId, CancellationToken cancellationToken = default)
    {
        return _db.TeacherClassSubjectAllocations.AsNoTracking()
            .Include(x => x.Teacher)
            .FirstOrDefaultAsync(
                x => x.SubjectId == subjectId
                     && x.IsActive
                     && x.Class.ToLower() == className.ToLower()
                     && x.Section.ToLower() == section.ToLower(),
                cancellationToken);
    }

    public Task<bool> ExistsActiveAsync(int teacherId, int subjectId, string className, string section, CancellationToken cancellationToken = default)
    {
        return _db.TeacherClassSubjectAllocations.AsNoTracking().AnyAsync(
            x => x.TeacherId == teacherId
                 && x.SubjectId == subjectId
                 && x.IsActive
                 && x.Class.ToLower() == className.ToLower()
                 && x.Section.ToLower() == section.ToLower(),
            cancellationToken);
    }

    public Task<bool> ExistsDuplicateAsync(int teacherId, int subjectId, string className, string section, int? exceptId, CancellationToken cancellationToken = default)
    {
        var query = _db.TeacherClassSubjectAllocations.AsNoTracking()
            .Where(x => x.TeacherId == teacherId && x.SubjectId == subjectId && x.Class == className && x.Section == section);
        if (exceptId.HasValue) query = query.Where(x => x.Id != exceptId.Value);
        return query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(TeacherClassSubjectAllocation row, CancellationToken cancellationToken = default)
    {
        await _db.TeacherClassSubjectAllocations.AddAsync(row, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TeacherClassSubjectAllocation row, CancellationToken cancellationToken = default)
    {
        _db.TeacherClassSubjectAllocations.Update(row);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _db.TeacherClassSubjectAllocations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row == null) return false;
        _db.TeacherClassSubjectAllocations.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
