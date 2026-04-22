using Microsoft.EntityFrameworkCore;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;
using SchoolManagement.Infrastructure.Persistence;

namespace SchoolManagement.Infrastructure.Repositories;

public class FeeRepository : IFeeRepository
{
    private readonly ApplicationDbContext _db;

    public FeeRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<FeeInvoice>> GetPagedInvoicesAsync(int page, int pageSize, int? studentId, FeeInvoiceStatus? status, string? className, string? section, DateOnly? dueFrom, DateOnly? dueTo, CancellationToken cancellationToken = default)
    {
        var query = _db.FeeInvoices.AsNoTracking().Include(x => x.Student).AsQueryable();
        if (studentId.HasValue) query = query.Where(x => x.StudentId == studentId.Value);
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (!string.IsNullOrWhiteSpace(className)) query = query.Where(x => x.Student != null && x.Student.Class.ToLower() == className.Trim().ToLower());
        if (!string.IsNullOrWhiteSpace(section)) query = query.Where(x => x.Student != null && x.Student.Section.ToLower() == section.Trim().ToLower());
        if (dueFrom.HasValue) query = query.Where(x => x.DueDate >= dueFrom.Value);
        if (dueTo.HasValue) query = query.Where(x => x.DueDate <= dueTo.Value);

        var total = await query.CountAsync(cancellationToken);
        var data = await query
            .OrderByDescending(x => x.DueDate)
            .ThenBy(x => x.Id)
            .Skip(Math.Max(0, page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<FeeInvoice> { Total = total, Page = page, PageSize = pageSize, Data = data };
    }

    public Task<FeeInvoice?> GetInvoiceByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.FeeInvoices.Include(x => x.Student).FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddInvoiceAsync(FeeInvoice invoice, CancellationToken cancellationToken = default)
    {
        await _db.FeeInvoices.AddAsync(invoice, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateInvoiceAsync(FeeInvoice invoice, CancellationToken cancellationToken = default)
    {
        _db.FeeInvoices.Update(invoice);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteInvoiceAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _db.FeeInvoices.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (row == null) return false;
        _db.FeeInvoices.Remove(row);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task AddPaymentAsync(FeePayment payment, CancellationToken cancellationToken = default)
    {
        await _db.FeePayments.AddAsync(payment, cancellationToken);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<FeePayment>> ListPaymentsByInvoiceAsync(int feeInvoiceId, CancellationToken cancellationToken = default)
    {
        return await _db.FeePayments.AsNoTracking()
            .Where(x => x.FeeInvoiceId == feeInvoiceId)
            .OrderByDescending(x => x.PaidOn)
            .ThenByDescending(x => x.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<FeeReportSummaryDto> GetSummaryAsync(DateOnly? from, DateOnly? to, string? className, string? section, CancellationToken cancellationToken = default)
    {
        var invoices = _db.FeeInvoices.AsNoTracking().Include(x => x.Student).AsQueryable();
        if (from.HasValue) invoices = invoices.Where(x => x.DueDate >= from.Value);
        if (to.HasValue) invoices = invoices.Where(x => x.DueDate <= to.Value);
        if (!string.IsNullOrWhiteSpace(className)) invoices = invoices.Where(x => x.Student != null && x.Student.Class.ToLower() == className.Trim().ToLower());
        if (!string.IsNullOrWhiteSpace(section)) invoices = invoices.Where(x => x.Student != null && x.Student.Section.ToLower() == section.Trim().ToLower());

        var data = await invoices.ToListAsync(cancellationToken);
        var result = new FeeReportSummaryDto
        {
            TotalInvoiced = data.Sum(x => x.Amount),
            TotalDiscount = data.Sum(x => x.Discount),
            TotalNetPayable = data.Sum(x => x.Amount - x.Discount),
            TotalCollected = data.Sum(x => x.AmountPaid),
            PendingCount = data.Count(x => x.Status == FeeInvoiceStatus.Pending),
            PartialCount = data.Count(x => x.Status == FeeInvoiceStatus.Partial),
            PaidCount = data.Count(x => x.Status == FeeInvoiceStatus.Paid),
            OverdueCount = data.Count(x => x.Status == FeeInvoiceStatus.Overdue)
        };
        result.TotalDue = result.TotalNetPayable - result.TotalCollected;
        return result;
    }
}
