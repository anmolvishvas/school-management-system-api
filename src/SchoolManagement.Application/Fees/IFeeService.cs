using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Fees.Dtos;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Fees;

public interface IFeeService
{
    Task<PagedResult<FeeInvoiceDto>> GetInvoicesAsync(
        int page,
        int pageSize,
        int? studentId,
        FeeInvoiceStatus? status,
        string? className,
        string? section,
        DateOnly? dueFrom,
        DateOnly? dueTo,
        CancellationToken cancellationToken = default);

    Task<FeeInvoiceDto?> GetInvoiceByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<FeeInvoiceDto> CreateInvoiceAsync(CreateFeeInvoiceDto dto, CancellationToken cancellationToken = default);
    Task<FeeInvoiceDto?> UpdateInvoiceAsync(int id, UpdateFeeInvoiceDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteInvoiceAsync(int id, CancellationToken cancellationToken = default);

    Task<FeePaymentDto> AddPaymentAsync(int invoiceId, CreateFeePaymentDto dto, int? receivedByUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FeePaymentDto>> GetPaymentsAsync(int invoiceId, CancellationToken cancellationToken = default);

    Task<FeeReportSummaryDto> GetSummaryAsync(DateOnly? from, DateOnly? to, string? className, string? section, CancellationToken cancellationToken = default);
}
