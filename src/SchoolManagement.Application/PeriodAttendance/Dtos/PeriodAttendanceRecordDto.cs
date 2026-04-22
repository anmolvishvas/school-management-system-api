using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.PeriodAttendance.Dtos;

public class PeriodAttendanceRecordDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TeacherUserId { get; set; }
    public DateOnly Date { get; set; }
    public int HourNumber { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
