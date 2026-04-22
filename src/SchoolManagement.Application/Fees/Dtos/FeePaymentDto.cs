namespace SchoolManagement.Application.Fees.Dtos;

public class FeePaymentDto
{
    public int Id { get; set; }
    public int FeeInvoiceId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly PaidOn { get; set; }
    public string? Mode { get; set; }
    public string? ReferenceNo { get; set; }
    public int? ReceivedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
