namespace SchoolManagement.Domain.Entities;

public class TimetableEntry
{
    public int Id { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
    public bool IsActive { get; set; } = true;
}
