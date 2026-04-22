namespace SchoolManagement.Application.Courses.Dtos;

public class UpdateCourseSubjectDto
{
    public int SubjectId { get; set; }
    public bool IsActive { get; set; } = true;
}
