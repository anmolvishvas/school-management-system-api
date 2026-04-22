namespace SchoolManagement.Application.Fees.Dtos;

public class CreateFeePaymentDto
{
    public decimal Amount { get; set; }
    public DateOnly PaidOn { get; set; }
    public string? Mode { get; set; }
    public string? ReferenceNo { get; set; }
}
