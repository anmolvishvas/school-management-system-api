using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.Infrastructure.Repositories;

public class SubjectRepository : ISubjectRepository
{
    private readonly ApplicationDbContext _db;

    public SubjectRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Subject>> GetAllAsync(bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var query = _db.Subjects.AsNoTracking().AsQueryable();
        if (activeOnly == true) query = query.Where(s => s.IsActive);
        return await query.OrderBy(s => s.Name).ToListAsync(cancellationToken);
    }

    public Task<Subject?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);

    public Task<Subject?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        => _db.Subjects.AsNoTracking().FirstOrDefaultAsync(s => s.Name == name, cancellationToken);

    public async Task AddAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        await _db.Subjects.AddAsync(subject, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Subject subject, CancellationToken cancellationToken = default)
    {
        _db.Subjects.Update(subject);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _db.Subjects.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        if (row == null) return false;
        _db.Subjects.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
