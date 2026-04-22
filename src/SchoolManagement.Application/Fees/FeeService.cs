using AutoMapper;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Fees.Dtos;
using SchoolManagement.Domain.Entities;
using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Fees;

public class FeeService : IFeeService
{
    private readonly IFeeRepository _fees;
    private readonly IStudentRepository _students;
    private readonly IMapper _mapper;

    public FeeService(IFeeRepository fees, IStudentRepository students, IMapper mapper)
    {
        _fees = fees;
        _students = students;
        _mapper = mapper;
    }

    public async Task<PagedResult<FeeInvoiceDto>> GetInvoicesAsync(int page, int pageSize, int? studentId, FeeInvoiceStatus? status, string? className, string? section, DateOnly? dueFrom, DateOnly? dueTo, CancellationToken cancellationToken = default)
    {
        var rows = await _fees.GetPagedInvoicesAsync(page, pageSize, studentId, status, className, section, dueFrom, dueTo, cancellationToken);
        var data = _mapper.Map<IReadOnlyList<FeeInvoiceDto>>(rows.Data);
        return new PagedResult<FeeInvoiceDto> { Total = rows.Total, Page = rows.Page, PageSize = rows.PageSize, Data = data };
    }

    public async Task<FeeInvoiceDto?> GetInvoiceByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var row = await _fees.GetInvoiceByIdAsync(id, cancellationToken);
        return row == null ? null : _mapper.Map<FeeInvoiceDto>(row);
    }

    public async Task<FeeInvoiceDto> CreateInvoiceAsync(CreateFeeInvoiceDto dto, CancellationToken cancellationToken = default)
    {
        _ = await _students.GetByIdAsync(dto.StudentId, cancellationToken) ?? throw new InvalidOperationException("Student not found.");
        if (dto.Discount > dto.Amount)
            throw new InvalidOperationException("Discount cannot exceed amount.");

        var row = new FeeInvoice
        {
            StudentId = dto.StudentId,
            Title = dto.Title.Trim(),
            DueDate = dto.DueDate,
            Amount = dto.Amount,
            Discount = dto.Discount,
            AmountPaid = 0,
            Notes = dto.Notes,
            CreatedAtUtc = DateTime.UtcNow
        };
        row.Status = ComputeStatus(row);

        await _fees.AddInvoiceAsync(row, cancellationToken);
        var created = await _fees.GetInvoiceByIdAsync(row.Id, cancellationToken) ?? row;
        return _mapper.Map<FeeInvoiceDto>(created);
    }

    public async Task<FeeInvoiceDto?> UpdateInvoiceAsync(int id, UpdateFeeInvoiceDto dto, CancellationToken cancellationToken = default)
    {
        var row = await _fees.GetInvoiceByIdAsync(id, cancellationToken);
        if (row == null) return null;

        if (dto.Discount > dto.Amount)
            throw new InvalidOperationException("Discount cannot exceed amount.");
        if (row.AmountPaid > dto.Amount - dto.Discount)
            throw new InvalidOperationException("Amount paid exceeds new net payable.");

        row.Title = dto.Title.Trim();
        row.DueDate = dto.DueDate;
        row.Amount = dto.Amount;
        row.Discount = dto.Discount;
        row.Notes = dto.Notes;
        row.Status = ComputeStatus(row);

        await _fees.UpdateInvoiceAsync(row, cancellationToken);
        var updated = await _fees.GetInvoiceByIdAsync(id, cancellationToken) ?? row;
        return _mapper.Map<FeeInvoiceDto>(updated);
    }

    public Task<bool> DeleteInvoiceAsync(int id, CancellationToken cancellationToken = default)
        => _fees.DeleteInvoiceAsync(id, cancellationToken);

    public async Task<FeePaymentDto> AddPaymentAsync(int invoiceId, CreateFeePaymentDto dto, int? receivedByUserId, CancellationToken cancellationToken = default)
    {
        var invoice = await _fees.GetInvoiceByIdAsync(invoiceId, cancellationToken) ?? throw new InvalidOperationException("Invoice not found.");
        var due = invoice.Amount - invoice.Discount - invoice.AmountPaid;
        if (due <= 0)
            throw new InvalidOperationException("Invoice is already fully paid.");
        if (dto.Amount > due)
            throw new InvalidOperationException("Payment amount exceeds pending due.");

        var payment = new FeePayment
        {
            FeeInvoiceId = invoiceId,
            Amount = dto.Amount,
            PaidOn = dto.PaidOn,
            Mode = dto.Mode,
            ReferenceNo = dto.ReferenceNo,
            ReceivedByUserId = receivedByUserId,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _fees.AddPaymentAsync(payment, cancellationToken);

        invoice.AmountPaid += dto.Amount;
        invoice.Status = ComputeStatus(invoice);
        await _fees.UpdateInvoiceAsync(invoice, cancellationToken);

        return _mapper.Map<FeePaymentDto>(payment);
    }

    public async Task<IReadOnlyList<FeePaymentDto>> GetPaymentsAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        _ = await _fees.GetInvoiceByIdAsync(invoiceId, cancellationToken) ?? throw new InvalidOperationException("Invoice not found.");
        var rows = await _fees.ListPaymentsByInvoiceAsync(invoiceId, cancellationToken);
        return _mapper.Map<IReadOnlyList<FeePaymentDto>>(rows);
    }

    public Task<FeeReportSummaryDto> GetSummaryAsync(DateOnly? from, DateOnly? to, string? className, string? section, CancellationToken cancellationToken = default)
        => _fees.GetSummaryAsync(from, to, className, section, cancellationToken);

    private static FeeInvoiceStatus ComputeStatus(FeeInvoice invoice)
    {
        var net = invoice.Amount - invoice.Discount;
        if (invoice.AmountPaid >= net) return FeeInvoiceStatus.Paid;
        if (invoice.AmountPaid > 0) return FeeInvoiceStatus.Partial;
        return invoice.DueDate < DateOnly.FromDateTime(DateTime.UtcNow) ? FeeInvoiceStatus.Overdue : FeeInvoiceStatus.Pending;
    }
}
