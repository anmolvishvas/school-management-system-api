namespace SchoolManagement.Application.Teachers.Dtos;

public class TeacherAllocationDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public bool IsClassTeacher { get; set; }
    public bool IsActive { get; set; }
}
