using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Fees.Dtos;

public class FeeInvoiceDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public decimal Amount { get; set; }
    public decimal Discount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal AmountDue { get; set; }
    public FeeInvoiceStatus Status { get; set; }
    public string? Notes { get; set; }
}
