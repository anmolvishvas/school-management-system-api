namespace SchoolManagement.Application.Common.Models;

public class AttendanceSummaryDto
{
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
    public int TotalMarks { get; set; }
    public int Present { get; set; }
    public int Absent { get; set; }
    public int Late { get; set; }
    public int Excused { get; set; }
    public int HalfDay { get; set; }
    public decimal? AttendanceRatePercent { get; set; }
}
