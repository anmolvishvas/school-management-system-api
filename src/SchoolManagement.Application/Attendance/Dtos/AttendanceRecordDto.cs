using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Application.Attendance.Dtos;

public class AttendanceRecordDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int? MarkedByUserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
