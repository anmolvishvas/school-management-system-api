namespace SchoolManagement.Application.Courses.Dtos;

public class CreateCourseSubjectDto
{
    public int SubjectId { get; set; }
    public bool IsActive { get; set; } = true;
}
