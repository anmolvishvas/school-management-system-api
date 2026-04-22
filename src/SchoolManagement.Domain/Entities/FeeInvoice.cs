using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class FeeInvoice
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public string Title { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public decimal Amount { get; set; }
    public decimal Discount { get; set; }
    public decimal AmountPaid { get; set; }
    public FeeInvoiceStatus Status { get; set; } = FeeInvoiceStatus.Pending;
    public string? Notes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
