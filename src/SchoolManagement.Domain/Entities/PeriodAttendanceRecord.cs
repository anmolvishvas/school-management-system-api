using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class PeriodAttendanceRecord
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public int TeacherUserId { get; set; }
    public User? TeacherUser { get; set; }
    public DateOnly Date { get; set; }
    public int HourNumber { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int? MarkedByUserId { get; set; }
    public User? MarkedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
