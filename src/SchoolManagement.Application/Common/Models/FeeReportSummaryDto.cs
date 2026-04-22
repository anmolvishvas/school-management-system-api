namespace SchoolManagement.Application.Common.Models;

public class FeeReportSummaryDto
{
    public decimal TotalInvoiced { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalNetPayable { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal TotalDue { get; set; }
    public int PendingCount { get; set; }
    public int PartialCount { get; set; }
    public int PaidCount { get; set; }
    public int OverdueCount { get; set; }
}
