namespace SchoolManagement.Application.Fees.Dtos;

public class UpdateFeeInvoiceDto
{
    public string Title { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; }
    public decimal Amount { get; set; }
    public decimal Discount { get; set; }
    public string? Notes { get; set; }
}
