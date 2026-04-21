namespace SchoolManagement.Application.Common.Models;

public class DashboardKpiDto
{
    public int TotalStudents { get; set; }
    public decimal? AttendanceRatePercent { get; set; }
    public decimal? FeeCollectionThisMonth { get; set; }
}
