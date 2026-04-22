using SchoolManagement.Domain.Enums;

namespace SchoolManagement.Domain.Entities;

public class AttendanceRecord
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public Student? Student { get; set; }
    public DateOnly Date { get; set; }
    public AttendanceStatus Status { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int? MarkedByUserId { get; set; }
    public User? MarkedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
