using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.PeriodAttendance.Dtos;

public class PeriodAttendanceLineDto
{
    public int StudentId { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}
