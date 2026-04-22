namespace SchoolManagement.Application.Teachers.Dtos;

public class CreateTeacherAllocationDto
{
    public int TeacherId { get; set; }
    public int SubjectId { get; set; }
    public string Class { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public bool IsClassTeacher { get; set; }
    public bool IsActive { get; set; } = true;
}
