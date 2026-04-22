using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Attendance.Dtos;

public class BulkAttendanceLineDto
{
    public int StudentId { get; set; }
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}
