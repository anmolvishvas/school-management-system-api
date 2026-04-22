using SchoolManagement.Application.Common.Models;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Common.Interfaces;

public interface IFeeRepository
{
    Task<PagedResult<FeeInvoice>> GetPagedInvoicesAsync(
        int page,
        int pageSize,
        int? studentId,
        FeeInvoiceStatus? status,
        string? className,
        string? section,
        DateOnly? dueFrom,
        DateOnly? dueTo,
        CancellationToken cancellationToken = default);

    Task<FeeInvoice?> GetInvoiceByIdAsync(int id, CancellationToken cancellationToken = default);
    Task AddInvoiceAsync(FeeInvoice invoice, CancellationToken cancellationToken = default);
    Task UpdateInvoiceAsync(FeeInvoice invoice, CancellationToken cancellationToken = default);
    Task<bool> DeleteInvoiceAsync(int id, CancellationToken cancellationToken = default);

    Task AddPaymentAsync(FeePayment payment, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeePayment>> ListPaymentsByInvoiceAsync(int feeInvoiceId, CancellationToken cancellationToken = default);

    Task<FeeReportSummaryDto> GetSummaryAsync(DateOnly? from, DateOnly? to, string? className, string? section, CancellationToken cancellationToken = default);
}
