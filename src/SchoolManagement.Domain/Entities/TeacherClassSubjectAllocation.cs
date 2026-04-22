namespace SchoolManagement.Domain.Entities;

public class TeacherClassSubjectAllocation
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
    public int SubjectId { get; set; }
    public Subject? Subject { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public bool IsClassTeacher { get; set; }
    public bool IsActive { get; set; } = true;
}
