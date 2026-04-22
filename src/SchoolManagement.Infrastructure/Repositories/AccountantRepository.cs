using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.Infrastructure.Repositories;

public class AccountantRepository : IAccountantRepository
{
    private readonly ApplicationDbContext _db;

    public AccountantRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Accountant>> GetAllAsync(bool? activeOnly, CancellationToken cancellationToken = default)
    {
        var query = _db.Accountants.AsNoTracking().Include(x => x.User).AsQueryable();
        if (activeOnly == true) query = query.Where(x => x.IsActive);
        return await query.OrderBy(x => x.FullName).ToListAsync(cancellationToken);
    }

    public Task<Accountant?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.Accountants.AsNoTracking().Include(x => x.User).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public Task<Accountant?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
        => _db.Accountants.AsNoTracking().Include(x => x.User).FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);

    public async Task AddAsync(Accountant row, CancellationToken cancellationToken = default)
    {
        await _db.Accountants.AddAsync(row, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Accountant row, CancellationToken cancellationToken = default)
    {
        _db.Accountants.Update(row);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _db.Accountants.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row == null) return false;
        _db.Accountants.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }
}
