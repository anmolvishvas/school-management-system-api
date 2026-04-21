using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.Infrastructure.Repositories;

public class StudentRepository : IStudentRepository
{
    private readonly ApplicationDbContext _db;

    public StudentRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<Student>> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        string sortBy,
        string order,
        string? className,
        string? section,
        bool? activeOnly,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Students.AsNoTracking().AsQueryable();

        if (activeOnly == true)
            query = query.Where(s => s.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(s =>
                s.Name.Contains(term) ||
                s.Class.Contains(term) ||
                s.Section.Contains(term) ||
                (s.AdmissionNumber != null && s.AdmissionNumber.Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(className))
        {
            var c = className.Trim().ToLower();
            query = query.Where(s => s.Class.ToLower() == c);
        }

        if (!string.IsNullOrWhiteSpace(section))
        {
            var sec = section.Trim().ToLower();
            query = query.Where(s => s.Section.ToLower() == sec);
        }

        var isAsc = !string.Equals(order, "desc", StringComparison.OrdinalIgnoreCase);

        query = sortBy?.ToLowerInvariant() switch
        {
            "name" => isAsc ? query.OrderBy(s => s.Name) : query.OrderByDescending(s => s.Name),
            "class" => isAsc ? query.OrderBy(s => s.Class) : query.OrderByDescending(s => s.Class),
            "section" => isAsc ? query.OrderBy(s => s.Section) : query.OrderByDescending(s => s.Section),
            _ => isAsc ? query.OrderBy(s => s.Id) : query.OrderByDescending(s => s.Id)
        };

        var total = await query.CountAsync(cancellationToken);

        var data = await query
            .Skip(Math.Max(0, page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Student>
        {
            Total = total,
            Page = page,
            PageSize = pageSize,
            Data = data
        };
    }

    public Task<Student?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _db.Students.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task AddAsync(Student student, CancellationToken cancellationToken = default)
    {
        await _db.Students.AddAsync(student, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Student student, CancellationToken cancellationToken = default)
    {
        _db.Students.Update(student);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Students.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (entity == null)
            return false;

        _db.Students.Remove(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return _db.Students.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LabelCountDto>> CountByClassAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Students
            .AsNoTracking()
            .GroupBy(s => s.Class)
            .Select(g => new LabelCountDto { Label = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<LabelCountDto>> CountBySectionAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Students
            .AsNoTracking()
            .GroupBy(s => s.Section)
            .Select(g => new LabelCountDto { Label = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);
    }
}
