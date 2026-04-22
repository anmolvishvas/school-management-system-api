namespace SchoolManagement.Application.Courses.Dtos;

public class CourseSubjectDto
{
    public int Id { get; set; }
    public int CourseId { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
