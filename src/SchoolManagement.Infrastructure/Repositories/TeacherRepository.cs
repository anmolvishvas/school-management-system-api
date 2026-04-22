using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.Infrastructure.Repositories;

public class TeacherRepository : ITeacherRepository
{
    private readonly ApplicationDbContext _db;

    public TeacherRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Teacher>> GetAllAsync(bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var query = _db.Teachers.AsNoTracking().Include(t => t.User).AsQueryable();
        if (activeOnly == true) query = query.Where(t => t.IsActive);
        return await query.OrderBy(t => t.FullName).ToListAsync(cancellationToken);
    }

    public Task<Teacher?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.Teachers.AsNoTracking().Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public Task<Teacher?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => _db.Teachers.AsNoTracking().FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);

    public async Task AddAsync(Teacher teacher, CancellationToken cancellationToken = default)
    {
        await _db.Teachers.AddAsync(teacher, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Teacher teacher, CancellationToken cancellationToken = default)
    {
        _db.Teachers.Update(teacher);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _db.Teachers.FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
        if (row == null) return false;
        _db.Teachers.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
