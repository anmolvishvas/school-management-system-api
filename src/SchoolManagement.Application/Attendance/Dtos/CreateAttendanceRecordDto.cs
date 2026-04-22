using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Attendance.Dtos;

public class CreateAttendanceRecordDto
{
    public int StudentId { get; set; }
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
