using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Attendance.Dtos;

public class UpdateAttendanceRecordDto
{
    public AttendanceStatus Status { get; set; }
    public string? Notes { get; set; }
}
